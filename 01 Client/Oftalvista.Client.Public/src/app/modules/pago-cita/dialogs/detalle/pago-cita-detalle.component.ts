import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../shared/material.module';
@Component({
  selector: 'app-pago-cita-detalle',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './pago-cita-detalle.component.html',
})
export class PagoCitaDetalleComponent {
  constructor(
    public ref: MatDialogRef<PagoCitaDetalleComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {}
}
