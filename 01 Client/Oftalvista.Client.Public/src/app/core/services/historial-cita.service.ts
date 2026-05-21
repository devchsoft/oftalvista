import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { paginatedItemsResponse } from '../models/pagination.model';
import {
  historialCitaItemsDto,
  createHistorialCitaRequest,
  updateHistorialCitaRequest,
  historialCitaListRequest,
} from '../models/historial-cita.model';
@Injectable({ providedIn: 'root' })
export class HistorialCitaService {
  private url = `${environment.apiUrl}/historial-cita`;
  constructor(private http: HttpClient) {}
  listar(req: historialCitaListRequest): Observable<paginatedItemsResponse<historialCitaItemsDto>> {
    let p = new HttpParams().set('pageSize', req.pageSize).set('skip', req.skip);
    if (req.sortField) p = p.set('sortField', req.sortField);
    if (req.sortDir) p = p.set('sortDir', req.sortDir);
    if (req.filter)
      Object.keys(req.filter).forEach((k) => {
        const v = (req.filter as any)[k];
        if (v != null && v !== '') p = p.set(k, v);
      });
    return this.http.get<paginatedItemsResponse<historialCitaItemsDto>>(this.url, { params: p });
  }
  obtener(guid: string): Observable<historialCitaItemsDto> {
    return this.http.get<historialCitaItemsDto>(`${this.url}/${guid}`);
  }
  crear(body: createHistorialCitaRequest): Observable<historialCitaItemsDto> {
    return this.http.post<historialCitaItemsDto>(this.url, body);
  }
  editar(guid: string, body: updateHistorialCitaRequest): Observable<historialCitaItemsDto> {
    return this.http.put<historialCitaItemsDto>(`${this.url}/${guid}`, body);
  }
  eliminar(guid: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${guid}`);
  }
}
