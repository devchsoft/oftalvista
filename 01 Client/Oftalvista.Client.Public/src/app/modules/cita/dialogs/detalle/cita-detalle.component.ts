import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../shared/material.module';
@Component({
  selector: 'app-cita-detalle',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './cita-detalle.component.html',
})
export class CitaDetalleComponent {
  constructor(
    public ref: MatDialogRef<CitaDetalleComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {}
}
