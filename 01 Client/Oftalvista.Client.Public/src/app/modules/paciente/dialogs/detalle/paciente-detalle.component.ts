import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../shared/material.module';
@Component({
  selector: 'app-paciente-detalle',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './paciente-detalle.component.html',
})
export class PacienteDetalleComponent {
  constructor(
    public ref: MatDialogRef<PacienteDetalleComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {}
}
