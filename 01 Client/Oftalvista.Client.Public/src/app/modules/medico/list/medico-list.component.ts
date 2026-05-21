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
import { CatalogoService } from '../../../core/services/catalogo.service';
import { MedicoService } from '../../../core/services/medico.service';
import { catalogoItem } from '../../../core/models/catalogo.model';
import { medicoItemsDto, medicoListRequest } from '../../../core/models/medico.model';
import { MedicoFormComponent } from '../dialogs/form/medico-form.component';
import { MedicoDetalleComponent } from '../dialogs/detalle/medico-detalle.component';
@Component({
  selector: 'app-medico-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MaterialModule,
    RouterModule,
    SidebarComponent,
    TopbarComponent,
  ],
  templateUrl: './medico-list.component.html',
})
export class MedicoListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  cols = ['rowNum', 'cmp', 'idTblEstadoVigencia', 'acciones'];
  data: medicoItemsDto[] = [];
  total = 0;
  loading = false;
  pageSize = 10;
  usuarios: catalogoItem[] = [];
  especialidades: catalogoItem[] = [];
  estadosVigencia: catalogoItem[] = [];
  filtros!: FormGroup;
  constructor(
    private svc: MedicoService,
    private dialog: MatDialog,
    private snack: MatSnackBar,
    private fb: FormBuilder,
    private cat: CatalogoService,
  ) {}
  ngOnInit(): void {
    this.cat.getUsuarios().subscribe((r) => (this.usuarios = r));
    this.cat.getEspecialidades().subscribe((r) => (this.especialidades = r));
    this.cat.getEstadosVigencia().subscribe((r) => (this.estadosVigencia = r));
    this.filtros = this.fb.group({ cmp: [''], fechaRegistroDesde: [''], fechaRegistroHasta: [''] });
    this.cargar();
  }
  cargar(skip = 0): void {
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
      .open(MedicoFormComponent, { width: '640px', data: null })
      .afterClosed()
      .subscribe((r) => {
        if (r) {
          this.snack.open('Creado correctamente', '', { duration: 3000 });
          this.cargar();
        }
      });
  }
  editar(row: medicoItemsDto): void {
    this.dialog
      .open(MedicoFormComponent, { width: '640px', data: row })
      .afterClosed()
      .subscribe((r) => {
        if (r) {
          this.snack.open('Actualizado correctamente', '', { duration: 3000 });
          this.cargar();
        }
      });
  }
  ver(row: medicoItemsDto): void {
    this.dialog.open(MedicoDetalleComponent, { width: '580px', data: row });
  }
  eliminar(row: medicoItemsDto): void {
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
