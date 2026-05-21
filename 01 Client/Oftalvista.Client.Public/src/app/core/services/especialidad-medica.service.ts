import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { paginatedItemsResponse } from '../models/pagination.model';
import {
  especialidadMedicaItemsDto,
  createEspecialidadMedicaRequest,
  updateEspecialidadMedicaRequest,
  especialidadMedicaListRequest,
} from '../models/especialidad-medica.model';
@Injectable({ providedIn: 'root' })
export class EspecialidadMedicaService {
  private url = `${environment.apiUrl}/especialidad-medica`;
  constructor(private http: HttpClient) {}
  listar(
    req: especialidadMedicaListRequest,
  ): Observable<paginatedItemsResponse<especialidadMedicaItemsDto>> {
    let p = new HttpParams().set('pageSize', req.pageSize).set('skip', req.skip);
    if (req.sortField) p = p.set('sortField', req.sortField);
    if (req.sortDir) p = p.set('sortDir', req.sortDir);
    if (req.filter)
      Object.keys(req.filter).forEach((k) => {
        const v = (req.filter as any)[k];
        if (v != null && v !== '') p = p.set(k, v);
      });
    return this.http.get<paginatedItemsResponse<especialidadMedicaItemsDto>>(this.url, {
      params: p,
    });
  }
  obtener(guid: string): Observable<especialidadMedicaItemsDto> {
    return this.http.get<especialidadMedicaItemsDto>(`${this.url}/${guid}`);
  }
  crear(body: createEspecialidadMedicaRequest): Observable<especialidadMedicaItemsDto> {
    return this.http.post<especialidadMedicaItemsDto>(this.url, body);
  }
  editar(
    guid: string,
    body: updateEspecialidadMedicaRequest,
  ): Observable<especialidadMedicaItemsDto> {
    return this.http.put<especialidadMedicaItemsDto>(`${this.url}/${guid}`, body);
  }
  eliminar(guid: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${guid}`);
  }
}
