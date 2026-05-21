import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { paginatedItemsResponse } from '../models/pagination.model';
import {
  pagoCitaItemsDto,
  createPagoCitaRequest,
  updatePagoCitaRequest,
  pagoCitaListRequest,
} from '../models/pago-cita.model';
@Injectable({ providedIn: 'root' })
export class PagoCitaService {
  private url = `${environment.apiUrl}/pago-cita`;
  constructor(private http: HttpClient) {}
  listar(req: pagoCitaListRequest): Observable<paginatedItemsResponse<pagoCitaItemsDto>> {
    let p = new HttpParams().set('pageSize', req.pageSize).set('skip', req.skip);
    if (req.sortField) p = p.set('sortField', req.sortField);
    if (req.sortDir) p = p.set('sortDir', req.sortDir);
    if (req.filter)
      Object.keys(req.filter).forEach((k) => {
        const v = (req.filter as any)[k];
        if (v != null && v !== '') p = p.set(k, v);
      });
    return this.http.get<paginatedItemsResponse<pagoCitaItemsDto>>(this.url, { params: p });
  }
  obtener(guid: string): Observable<pagoCitaItemsDto> {
    return this.http.get<pagoCitaItemsDto>(`${this.url}/${guid}`);
  }
  crear(body: createPagoCitaRequest): Observable<pagoCitaItemsDto> {
    return this.http.post<pagoCitaItemsDto>(this.url, body);
  }
  editar(guid: string, body: updatePagoCitaRequest): Observable<pagoCitaItemsDto> {
    return this.http.put<pagoCitaItemsDto>(`${this.url}/${guid}`, body);
  }
  eliminar(guid: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${guid}`);
  }
}
