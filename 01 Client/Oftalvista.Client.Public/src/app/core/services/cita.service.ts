import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { paginatedItemsResponse } from '../models/pagination.model';
import {
  citaItemsDto,
  createCitaRequest,
  updateCitaRequest,
  citaListRequest,
} from '../models/cita.model';
@Injectable({ providedIn: 'root' })
export class CitaService {
  private url = `${environment.apiUrl}/cita`;
  constructor(private http: HttpClient) {}
  listar(req: citaListRequest): Observable<paginatedItemsResponse<citaItemsDto>> {
    let p = new HttpParams().set('pageSize', req.pageSize).set('skip', req.skip);
    if (req.sortField) p = p.set('sortField', req.sortField);
    if (req.sortDir) p = p.set('sortDir', req.sortDir);
    if (req.filter)
      Object.keys(req.filter).forEach((k) => {
        const v = (req.filter as any)[k];
        if (v != null && v !== '') p = p.set(k, v);
      });
    return this.http.get<paginatedItemsResponse<citaItemsDto>>(this.url, { params: p });
  }
  obtener(guid: string): Observable<citaItemsDto> {
    return this.http.get<citaItemsDto>(`${this.url}/${guid}`);
  }
  crear(body: createCitaRequest): Observable<citaItemsDto> {
    return this.http.post<citaItemsDto>(this.url, body);
  }
  editar(guid: string, body: updateCitaRequest): Observable<citaItemsDto> {
    return this.http.put<citaItemsDto>(`${this.url}/${guid}`, body);
  }
  eliminar(guid: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${guid}`);
  }
}
