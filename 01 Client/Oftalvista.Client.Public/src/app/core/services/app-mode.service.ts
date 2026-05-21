import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AppModeService {
  private readonly storageKey = 'ov_use_demo';

  isDemoMode(): boolean {
    const stored = localStorage.getItem(this.storageKey);
    if (stored == null) return environment.demoMode;
    return stored === 'true';
  }

  setDemoMode(enabled: boolean): void {
    localStorage.setItem(this.storageKey, String(enabled));
  }
}
