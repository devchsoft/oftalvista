import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../shared/material.module';
@Component({
  selector: 'app-historial-cita-detalle',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './historial-cita-detalle.component.html',
})
export class HistorialCitaDetalleComponent {
  constructor(
    public ref: MatDialogRef<HistorialCitaDetalleComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {}
}
