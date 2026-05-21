import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RouterModule } from '@angular/router';
import { MaterialModule } from '../../../shared/material.module';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { TopbarComponent } from '../../../shared/components/topbar/topbar.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { AuthService } from '../../../core/services/auth.service';
import { CatalogoService } from '../../../core/services/catalogo.service';
import { CitaService } from '../../../core/services/cita.service';
import { UserScopeService } from '../../../core/services/user-scope.service';
import { catalogoItem } from '../../../core/models/catalogo.model';
import { citaItemsDto, citaListRequest } from '../../../core/models/cita.model';
import { CitaFormComponent } from '../dialogs/form/cita-form.component';
import { CitaDetalleComponent } from '../dialogs/detalle/cita-detalle.component';
@Component({
  selector: 'app-cita-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MaterialModule,
    RouterModule,
    SidebarComponent,
    TopbarComponent,
  ],
  templateUrl: './cita-list.component.html',
})
export class CitaListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  cols = ['rowNum', 'fechaCita', 'horaCita', 'motivo', 'idTblEstadoVigencia', 'acciones'];
  data: citaItemsDto[] = [];
  total = 0;
  loading = false;
  pageSize = 10;
  pacientes: catalogoItem[] = [];
  medicos: catalogoItem[] = [];
  estadosCita: catalogoItem[] = [];
  modalidades: catalogoItem[] = [];
  estadosVigencia: catalogoItem[] = [];
  filtros!: FormGroup;
  currentPacienteId: number | null = null;
  constructor(
    private svc: CitaService,
    private dialog: MatDialog,
    private snack: MatSnackBar,
    private fb: FormBuilder,
    private cat: CatalogoService,
    private auth: AuthService,
    private userScope: UserScopeService,
  ) {}
  get isAdmin(): boolean {
    return this.auth.isAdmin();
  }
  ngOnInit(): void {
    if (this.isAdmin) this.cat.getPacientes().subscribe((r) => (this.pacientes = r));
    this.cat.getMedicos().subscribe((r) => (this.medicos = r));
    this.cat.getEstadosCita().subscribe((r) => (this.estadosCita = r));
    this.cat.getModalidadesCita().subscribe((r) => (this.modalidades = r));
    this.cat.getEstadosVigencia().subscribe((r) => (this.estadosVigencia = r));
    this.filtros = this.fb.group({ fechaRegistroDesde: [''], fechaRegistroHasta: [''] });
    this.inicializarContexto();
  }
  inicializarContexto(): void {
    if (this.isAdmin) {
      this.cargar();
      return;
    }
    this.userScope.getCurrentPaciente().subscribe((paciente) => {
      this.currentPacienteId = paciente?.idPaciente ?? null;
      this.cargar();
    });
  }
  cargar(skip = 0): void {
    this.loading = true;
    this.svc
      .listar({
        pageSize: this.pageSize,
        skip,
        sortField: 'Id',
        sortDir: 'asc',
        filter: this.buildFilter(),
      })
      .subscribe({
        next: (r) => {
          this.data = r.data;
          this.total = r.count;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        },
      });
  }
  buildFilter(): any {
    const filter = { ...(this.filtros.value as any) };
    if (!this.isAdmin && this.currentPacienteId) filter.idPaciente = String(this.currentPacienteId);
    return filter;
  }
  buscar(): void {
    if (this.paginator) this.paginator.firstPage();
    this.cargar(0);
  }
  limpiar(): void {
    this.filtros.reset();
    this.buscar();
  }
  onPage(e: any): void {
    this.pageSize = e.pageSize;
    this.cargar(e.pageIndex * e.pageSize);
  }
  nuevo(): void {
    this.dialog
      .open(CitaFormComponent, { width: '640px', data: null })
      .afterClosed()
      .subscribe((r) => {
        if (r) {
          this.snack.open('Creado correctamente', '', { duration: 3000 });
          this.cargar();
        }
      });
  }
  editar(row: citaItemsDto): void {
    if (!this.isAdmin) return;
    this.dialog
      .open(CitaFormComponent, { width: '640px', data: row })
      .afterClosed()
      .subscribe((r) => {
        if (r) {
          this.snack.open('Actualizado correctamente', '', { duration: 3000 });
          this.cargar();
        }
      });
  }
  ver(row: citaItemsDto): void {
    this.dialog.open(CitaDetalleComponent, { width: '580px', data: row });
  }
  eliminar(row: citaItemsDto): void {
    if (!this.isAdmin) return;
    this.dialog
      .open(ConfirmDialogComponent, {
        width: '420px',
        data: { title: 'Eliminar registro', message: 'Esta accion no se puede deshacer.' },
      })
      .afterClosed()
      .subscribe((ok) => {
        if (ok)
          this.svc.eliminar((row as any).guid ?? '').subscribe(() => {
            this.snack.open('Eliminado', '', { duration: 3000 });
            this.cargar();
          });
      });
  }
  labelEstado(id: number): string {
    return id === 1 ? 'Activo' : 'Inactivo';
  }
  chipEstado(id: number): string {
    return id === 1 ? 'chip chip-ok' : 'chip chip-off';
  }
}
