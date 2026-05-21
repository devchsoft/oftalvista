import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../shared/material.module';
@Component({
  selector: 'app-especialidad-medica-detalle',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './especialidad-medica-detalle.component.html',
})
export class EspecialidadMedicaDetalleComponent {
  constructor(
    public ref: MatDialogRef<EspecialidadMedicaDetalleComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {}
}
