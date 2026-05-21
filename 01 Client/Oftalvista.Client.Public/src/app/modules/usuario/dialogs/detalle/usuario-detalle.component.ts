import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../shared/material.module';
@Component({
  selector: 'app-usuario-detalle',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './usuario-detalle.component.html',
})
export class UsuarioDetalleComponent {
  constructor(
    public ref: MatDialogRef<UsuarioDetalleComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {}
}
