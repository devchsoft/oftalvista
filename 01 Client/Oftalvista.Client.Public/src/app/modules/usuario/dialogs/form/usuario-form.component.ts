import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../shared/material.module';
import { CatalogoService } from '../../../../core/services/catalogo.service';
import { UsuarioService } from '../../../../core/services/usuario.service';
import { catalogoItem } from '../../../../core/models/catalogo.model';
@Component({
  selector: 'app-usuario-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './usuario-form.component.html',
})
export class UsuarioFormComponent implements OnInit {
  esEdicion = false;
  loading = false;
  tiposUsuario: catalogoItem[] = [];
  tiposDocumento: catalogoItem[] = [];
  estadosVigencia: catalogoItem[] = [];
  form: FormGroup;
  constructor(
    private fb: FormBuilder,
    private svc: UsuarioService,
    private cat: CatalogoService,
    public ref: MatDialogRef<UsuarioFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    this.form = this.fb.group({
      idTipoUsuario: [null as any, [Validators.required]],
      idTipoDocumento: [null as any],
      numeroDocumento: [null as any, [Validators.required]],
      telefono: [null as any],
      nombres: [null as any, [Validators.required]],
      apellidos: [null as any, [Validators.required]],
      correo: [null as any, [Validators.required]],
      claveHash: [null as any],
      idTblEstadoVigencia: [null as any, [Validators.required]],
    });
  }
  ngOnInit(): void {
    this.cat.getTiposUsuario().subscribe((r) => (this.tiposUsuario = r));
    this.cat.getTiposDocumento().subscribe((r) => (this.tiposDocumento = r));
    this.cat.getEstadosVigencia().subscribe((r) => (this.estadosVigencia = r));
    if (this.data) {
      this.esEdicion = true;
      this.form.patchValue(this.data);
    }
  }
  guardar(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) return;
    this.loading = true;
    const body = this.form.value;
    const op = this.esEdicion
      ? this.svc.editar(this.data.guid ?? '', { ...body, guidUsuario: this.data.guid } as any)
      : this.svc.crear(body as any);
    op.subscribe({
      next: () => {
        this.loading = false;
        this.ref.close(true);
      },
      error: () => {
        this.loading = false;
      },
    });
  }
}
