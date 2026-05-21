import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MaterialModule } from '../../../shared/material.module';
import { AuthService } from '../../../core/services/auth.service';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  hidePass = true;
  loading = false;
  form: FormGroup;
  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
  ) {
    this.form = this.fb.group({
      correo: ['', [Validators.required, Validators.email]],
      claveHash: ['', [Validators.required, Validators.minLength(6)]],
    });
  }
  get correo() {
    return this.form.get('correo');
  }
  get claveHash() {
    return this.form.get('claveHash');
  }
  submit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) return;
    this.loading = true;
    this.auth.login(this.form.value as any).subscribe({
      next: (r) => {
        this.loading = false;
        this.router.navigate([r.idTipoUsuario === 1 ? '/dashboard/admin' : '/dashboard/paciente']);
      },
      error: () => {
        this.loading = false;
      },
    });
  }
}
