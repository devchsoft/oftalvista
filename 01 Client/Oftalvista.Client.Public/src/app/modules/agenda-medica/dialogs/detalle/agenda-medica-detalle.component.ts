import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../shared/material.module';
@Component({
  selector: 'app-agenda-medica-detalle',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './agenda-medica-detalle.component.html',
})
export class AgendaMedicaDetalleComponent {
  constructor(
    public ref: MatDialogRef<AgendaMedicaDetalleComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {}
}
