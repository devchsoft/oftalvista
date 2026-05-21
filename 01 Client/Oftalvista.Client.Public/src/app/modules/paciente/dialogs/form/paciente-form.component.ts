import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../shared/material.module';
import { CatalogoService } from '../../../../core/services/catalogo.service';
import { PacienteService } from '../../../../core/services/paciente.service';
import { catalogoItem } from '../../../../core/models/catalogo.model';
@Component({
  selector: 'app-paciente-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './paciente-form.component.html',
})
export class PacienteFormComponent implements OnInit {
  esEdicion = false;
  loading = false;
  usuarios: catalogoItem[] = [];
  estadosVigencia: catalogoItem[] = [];
  form: FormGroup;
  constructor(
    private fb: FormBuilder,
    private svc: PacienteService,
    private cat: CatalogoService,
    public ref: MatDialogRef<PacienteFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    this.form = this.fb.group({
      idUsuario: [null as any, [Validators.required]],
      fechaNacimiento: [null as any],
      sexo: [null as any, [Validators.required]],
      idTblEstadoVigencia: [null as any, [Validators.required]],
      direccion: [null as any],
      contactoEmergencia: [null as any],
      telefonoEmergencia: [null as any],
    });
  }
  ngOnInit(): void {
    this.cat.getUsuarios().subscribe((r) => (this.usuarios = r));
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
      ? this.svc.editar(this.data.guid ?? '', { ...body, guidPaciente: this.data.guid } as any)
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
