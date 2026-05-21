import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { paginatedItemsResponse } from '../models/pagination.model';
import {
  medicoItemsDto,
  createMedicoRequest,
  updateMedicoRequest,
  medicoListRequest,
} from '../models/medico.model';
@Injectable({ providedIn: 'root' })
export class MedicoService {
  private url = `${environment.apiUrl}/medico`;
  constructor(private http: HttpClient) {}
  listar(req: medicoListRequest): Observable<paginatedItemsResponse<medicoItemsDto>> {
    let p = new HttpParams().set('pageSize', req.pageSize).set('skip', req.skip);
    if (req.sortField) p = p.set('sortField', req.sortField);
    if (req.sortDir) p = p.set('sortDir', req.sortDir);
    if (req.filter)
      Object.keys(req.filter).forEach((k) => {
        const v = (req.filter as any)[k];
        if (v != null && v !== '') p = p.set(k, v);
      });
    return this.http.get<paginatedItemsResponse<medicoItemsDto>>(this.url, { params: p });
  }
  obtener(guid: string): Observable<medicoItemsDto> {
    return this.http.get<medicoItemsDto>(`${this.url}/${guid}`);
  }
  crear(body: createMedicoRequest): Observable<medicoItemsDto> {
    return this.http.post<medicoItemsDto>(this.url, body);
  }
  editar(guid: string, body: updateMedicoRequest): Observable<medicoItemsDto> {
    return this.http.put<medicoItemsDto>(`${this.url}/${guid}`, body);
  }
  eliminar(guid: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${guid}`);
  }
}
