import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AppModeService } from '../services/app-mode.service';
import { DemoBackendService } from '../services/demo-backend.service';
import { environment } from '../../../environments/environment';

export const demoInterceptor: HttpInterceptorFn = (req, next) => {
  const mode = inject(AppModeService);
  if (!mode.isDemoMode() || !req.url.startsWith(environment.apiUrl)) {
    return next(req);
  }

  return inject(DemoBackendService).handle(req);
};
