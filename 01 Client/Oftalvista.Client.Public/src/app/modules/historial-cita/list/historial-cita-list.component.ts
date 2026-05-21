import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { RouterModule } from '@angular/router';
import { MaterialModule } from '../../../shared/material.module';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { TopbarComponent } from '../../../shared/components/topbar/topbar.component';
import { AuthService } from '../../../core/services/auth.service';
import { CatalogoService } from '../../../core/services/catalogo.service';
import { CitaService } from '../../../core/services/cita.service';
import { HistorialCitaService } from '../../../core/services/historial-cita.service';
import { UserScopeService } from '../../../core/services/user-scope.service';
import { catalogoItem } from '../../../core/models/catalogo.model';
import {
  historialCitaItemsDto,
  historialCitaListRequest,
} from '../../../core/models/historial-cita.model';
import { HistorialCitaDetalleComponent } from '../dialogs/detalle/historial-cita-detalle.component';
@Component({
  selector: 'app-historial-cita-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MaterialModule,
    RouterModule,
    SidebarComponent,
    TopbarComponent,
  ],
  templateUrl: './historial-cita-list.component.html',
})
export class HistorialCitaListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  cols = ['rowNum', 'fechaEvento', 'descripcion', 'idTblEstadoVigencia', 'acciones'];
  data: historialCitaItemsDto[] = [];
  total = 0;
  loading = false;
  pageSize = 10;
  citas: catalogoItem[] = [];
  estadosCita: catalogoItem[] = [];
  estadosVigencia: catalogoItem[] = [];
  filtros!: FormGroup;
  currentPacienteId: number | null = null;
  allPatientHistory: historialCitaItemsDto[] = [];
  constructor(
    private svc: HistorialCitaService,
    private citaService: CitaService,
    private dialog: MatDialog,
    private fb: FormBuilder,
    private cat: CatalogoService,
    private auth: AuthService,
    private userScope: UserScopeService,
  ) {}
  get isAdmin(): boolean {
    return this.auth.isAdmin();
  }
  ngOnInit(): void {
    if (this.isAdmin) this.cat.getCitas().subscribe((r) => (this.citas = r));
    this.cat.getEstadosCita().subscribe((r) => (this.estadosCita = r));
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
    if (!this.isAdmin) {
      this.cargarHistorialPaciente(skip);
      return;
    }
    this.loading = true;
    this.svc
      .listar({
        pageSize: this.pageSize,
        skip,
        sortField: 'Id',
        sortDir: 'asc',
        filter: this.filtros.value as any,
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
  cargarHistorialPaciente(skip = 0): void {
    if (!this.currentPacienteId) {
      this.data = [];
      this.total = 0;
      return;
    }
    this.loading = true;
    this.citaService
      .listar({
        pageSize: 500,
        skip: 0,
        sortField: 'Id',
        sortDir: 'asc',
        filter: { idPaciente: String(this.currentPacienteId) } as any,
      })
      .subscribe({
        next: (citasResponse) => {
          const citaIds = new Set(citasResponse.data.map((item) => item.idCita));
          this.svc
            .listar({
              pageSize: 500,
              skip: 0,
              sortField: 'Id',
              sortDir: 'asc',
              filter: this.filtros.value as any,
            })
            .subscribe({
              next: (historialResponse) => {
                this.allPatientHistory = historialResponse.data.filter((item) =>
                  citaIds.has(item.idCita),
                );
                this.total = this.allPatientHistory.length;
                this.data = this.allPatientHistory
                  .slice(skip, skip + this.pageSize)
                  .map((item, index) => ({ ...item, rowNum: skip + index + 1 }));
                this.loading = false;
              },
              error: () => {
                this.loading = false;
              },
            });
        },
        error: () => {
          this.loading = false;
        },
      });
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
  ver(row: historialCitaItemsDto): void {
    this.dialog.open(HistorialCitaDetalleComponent, { width: '580px', data: row });
  }
  labelEstado(id: number): string {
    return id === 1 ? 'Activo' : 'Inactivo';
  }
  chipEstado(id: number): string {
    return id === 1 ? 'chip chip-ok' : 'chip chip-off';
  }
}
