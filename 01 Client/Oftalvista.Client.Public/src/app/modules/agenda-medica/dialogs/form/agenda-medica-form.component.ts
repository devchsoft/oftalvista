import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../shared/material.module';
import { CatalogoService } from '../../../../core/services/catalogo.service';
import { AgendaMedicaService } from '../../../../core/services/agenda-medica.service';
import { catalogoItem } from '../../../../core/models/catalogo.model';
@Component({
  selector: 'app-agenda-medica-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './agenda-medica-form.component.html',
})
export class AgendaMedicaFormComponent implements OnInit {
  esEdicion = false;
  loading = false;
  medicos: catalogoItem[] = [];
  estadosVigencia: catalogoItem[] = [];
  form: FormGroup;
  constructor(
    private fb: FormBuilder,
    private svc: AgendaMedicaService,
    private cat: CatalogoService,
    public ref: MatDialogRef<AgendaMedicaFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    this.form = this.fb.group({
      idMedico: [null as any, [Validators.required]],
      fecha: [null as any, [Validators.required]],
      idTblEstadoVigencia: [null as any, [Validators.required]],
      horaInicio: [null as any, [Validators.required]],
      horaFin: [null as any, [Validators.required]],
      esDisponible: [null as any],
      observacion: [null as any],
    });
  }
  ngOnInit(): void {
    this.cat.getMedicos().subscribe((r) => (this.medicos = r));
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
      ? this.svc.editar(this.data.guid ?? '', { ...body, guidAgendaMedica: this.data.guid } as any)
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
