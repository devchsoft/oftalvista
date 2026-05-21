import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { paginatedItemsResponse } from '../models/pagination.model';
import {
  agendaMedicaItemsDto,
  createAgendaMedicaRequest,
  updateAgendaMedicaRequest,
  agendaMedicaListRequest,
} from '../models/agenda-medica.model';
@Injectable({ providedIn: 'root' })
export class AgendaMedicaService {
  private url = `${environment.apiUrl}/agenda-medica`;
  constructor(private http: HttpClient) {}
  listar(req: agendaMedicaListRequest): Observable<paginatedItemsResponse<agendaMedicaItemsDto>> {
    let p = new HttpParams().set('pageSize', req.pageSize).set('skip', req.skip);
    if (req.sortField) p = p.set('sortField', req.sortField);
    if (req.sortDir) p = p.set('sortDir', req.sortDir);
    if (req.filter)
      Object.keys(req.filter).forEach((k) => {
        const v = (req.filter as any)[k];
        if (v != null && v !== '') p = p.set(k, v);
      });
    return this.http.get<paginatedItemsResponse<agendaMedicaItemsDto>>(this.url, { params: p });
  }
  obtener(guid: string): Observable<agendaMedicaItemsDto> {
    return this.http.get<agendaMedicaItemsDto>(`${this.url}/${guid}`);
  }
  crear(body: createAgendaMedicaRequest): Observable<agendaMedicaItemsDto> {
    return this.http.post<agendaMedicaItemsDto>(this.url, body);
  }
  editar(guid: string, body: updateAgendaMedicaRequest): Observable<agendaMedicaItemsDto> {
    return this.http.put<agendaMedicaItemsDto>(`${this.url}/${guid}`, body);
  }
  eliminar(guid: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${guid}`);
  }
}
