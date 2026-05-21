import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../material.module';
import { AuthService } from '../../../core/services/auth.service';
@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.scss'],
})
export class TopbarComponent {
  @Input() title = '';
  @Input() subtitle = '';
  constructor(public auth: AuthService) {}
}
