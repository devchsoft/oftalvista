import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../shared/material.module';
import { CatalogoService } from '../../../../core/services/catalogo.service';
import { EspecialidadMedicaService } from '../../../../core/services/especialidad-medica.service';
import { catalogoItem } from '../../../../core/models/catalogo.model';
@Component({
  selector: 'app-especialidad-medica-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './especialidad-medica-form.component.html',
})
export class EspecialidadMedicaFormComponent implements OnInit {
  esEdicion = false;
  loading = false;
  estadosVigencia: catalogoItem[] = [];
  form: FormGroup;
  constructor(
    private fb: FormBuilder,
    private svc: EspecialidadMedicaService,
    private cat: CatalogoService,
    public ref: MatDialogRef<EspecialidadMedicaFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    this.form = this.fb.group({
      codigo: [null as any, [Validators.required]],
      idTblEstadoVigencia: [null as any, [Validators.required]],
      nombre: [null as any, [Validators.required]],
      descripcion: [null as any],
    });
  }
  ngOnInit(): void {
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
      ? this.svc.editar(this.data.guid ?? '', {
          ...body,
          guidEspecialidadMedica: this.data.guid,
        } as any)
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
