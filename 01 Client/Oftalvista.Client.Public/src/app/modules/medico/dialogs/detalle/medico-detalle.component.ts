import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../shared/material.module';
@Component({
  selector: 'app-medico-detalle',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './medico-detalle.component.html',
})
export class MedicoDetalleComponent {
  constructor(
    public ref: MatDialogRef<MedicoDetalleComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {}
}
