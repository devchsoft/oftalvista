import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../shared/material.module';
import { CatalogoService } from '../../../../core/services/catalogo.service';
import { PagoCitaService } from '../../../../core/services/pago-cita.service';
import { catalogoItem } from '../../../../core/models/catalogo.model';
@Component({
  selector: 'app-pago-cita-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './pago-cita-form.component.html',
})
export class PagoCitaFormComponent implements OnInit {
  esEdicion = false;
  loading = false;
  citas: catalogoItem[] = [];
  metodosPago: catalogoItem[] = [];
  estadosPago: catalogoItem[] = [];
  estadosVigencia: catalogoItem[] = [];
  form: FormGroup;
  constructor(
    private fb: FormBuilder,
    private svc: PagoCitaService,
    private cat: CatalogoService,
    public ref: MatDialogRef<PagoCitaFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    this.form = this.fb.group({
      idCita: [null as any, [Validators.required]],
      idMetodoPago: [null as any, [Validators.required]],
      idEstadoPago: [null as any, [Validators.required]],
      monto: [null as any, [Validators.required]],
      fechaPago: [null as any],
      idTblEstadoVigencia: [null as any, [Validators.required]],
      numeroOperacion: [null as any],
      comprobante: [null as any],
    });
  }
  ngOnInit(): void {
    this.cat.getCitas().subscribe((r) => (this.citas = r));
    this.cat.getMetodosPago().subscribe((r) => (this.metodosPago = r));
    this.cat.getEstadosPago().subscribe((r) => (this.estadosPago = r));
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
      ? this.svc.editar(this.data.guid ?? '', { ...body, guidPagoCita: this.data.guid } as any)
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
