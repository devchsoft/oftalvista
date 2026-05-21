import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../shared/material.module';
import { CatalogoService } from '../../../../core/services/catalogo.service';
import { HistorialCitaService } from '../../../../core/services/historial-cita.service';
import { catalogoItem } from '../../../../core/models/catalogo.model';
@Component({
  selector: 'app-historial-cita-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './historial-cita-form.component.html',
})
export class HistorialCitaFormComponent implements OnInit {
  esEdicion = false;
  loading = false;
  citas: catalogoItem[] = [];
  estadosCita: catalogoItem[] = [];
  estadosVigencia: catalogoItem[] = [];
  form: FormGroup;
  constructor(
    private fb: FormBuilder,
    private svc: HistorialCitaService,
    private cat: CatalogoService,
    public ref: MatDialogRef<HistorialCitaFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    this.form = this.fb.group({
      idCita: [null as any, [Validators.required]],
      idEstadoCita: [null as any, [Validators.required]],
      fechaEvento: [null as any, [Validators.required]],
      idTblEstadoVigencia: [null as any, [Validators.required]],
      descripcion: [null as any],
    });
  }
  ngOnInit(): void {
    this.cat.getCitas().subscribe((r) => (this.citas = r));
    this.cat.getEstadosCita().subscribe((r) => (this.estadosCita = r));
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
      ? this.svc.editar(this.data.guid ?? '', { ...body, guidHistorialCita: this.data.guid } as any)
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
