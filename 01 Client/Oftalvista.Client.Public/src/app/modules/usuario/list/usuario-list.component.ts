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
import { UsuarioService } from '../../../core/services/usuario.service';
import { catalogoItem } from '../../../core/models/catalogo.model';
import { usuarioItemsDto, usuarioListRequest } from '../../../core/models/usuario.model';
import { UsuarioFormComponent } from '../dialogs/form/usuario-form.component';
import { UsuarioDetalleComponent } from '../dialogs/detalle/usuario-detalle.component';
@Component({
  selector: 'app-usuario-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MaterialModule,
    RouterModule,
    SidebarComponent,
    TopbarComponent,
  ],
  templateUrl: './usuario-list.component.html',
})
export class UsuarioListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  cols = [
    'rowNum',
    'nombres',
    'apellidos',
    'correo',
    'telefono',
    'idTblEstadoVigencia',
    'acciones',
  ];
  data: usuarioItemsDto[] = [];
  total = 0;
  loading = false;
  pageSize = 10;
  tiposUsuario: catalogoItem[] = [];
  tiposDocumento: catalogoItem[] = [];
  estadosVigencia: catalogoItem[] = [];
  filtros!: FormGroup;
  constructor(
    private svc: UsuarioService,
    private dialog: MatDialog,
    private snack: MatSnackBar,
    private fb: FormBuilder,
    private cat: CatalogoService,
  ) {}
  ngOnInit(): void {
    this.cat.getTiposUsuario().subscribe((r) => (this.tiposUsuario = r));
    this.cat.getTiposDocumento().subscribe((r) => (this.tiposDocumento = r));
    this.cat.getEstadosVigencia().subscribe((r) => (this.estadosVigencia = r));
    this.filtros = this.fb.group({
      numeroDocumento: [''],
      fechaRegistroDesde: [''],
      fechaRegistroHasta: [''],
    });
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
      .open(UsuarioFormComponent, { width: '640px', data: null })
      .afterClosed()
      .subscribe((r) => {
        if (r) {
          this.snack.open('Creado correctamente', '', { duration: 3000 });
          this.cargar();
        }
      });
  }
  editar(row: usuarioItemsDto): void {
    this.dialog
      .open(UsuarioFormComponent, { width: '640px', data: row })
      .afterClosed()
      .subscribe((r) => {
        if (r) {
          this.snack.open('Actualizado correctamente', '', { duration: 3000 });
          this.cargar();
        }
      });
  }
  ver(row: usuarioItemsDto): void {
    this.dialog.open(UsuarioDetalleComponent, { width: '580px', data: row });
  }
  eliminar(row: usuarioItemsDto): void {
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
