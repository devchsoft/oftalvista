import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { paginatedItemsResponse } from '../models/pagination.model';
import {
  pacienteItemsDto,
  createPacienteRequest,
  updatePacienteRequest,
  pacienteListRequest,
} from '../models/paciente.model';
@Injectable({ providedIn: 'root' })
export class PacienteService {
  private url = `${environment.apiUrl}/paciente`;
  constructor(private http: HttpClient) {}
  listar(req: pacienteListRequest): Observable<paginatedItemsResponse<pacienteItemsDto>> {
    let p = new HttpParams().set('pageSize', req.pageSize).set('skip', req.skip);
    if (req.sortField) p = p.set('sortField', req.sortField);
    if (req.sortDir) p = p.set('sortDir', req.sortDir);
    if (req.filter)
      Object.keys(req.filter).forEach((k) => {
        const v = (req.filter as any)[k];
        if (v != null && v !== '') p = p.set(k, v);
      });
    return this.http.get<paginatedItemsResponse<pacienteItemsDto>>(this.url, { params: p });
  }
  obtener(guid: string): Observable<pacienteItemsDto> {
    return this.http.get<pacienteItemsDto>(`${this.url}/${guid}`);
  }
  crear(body: createPacienteRequest): Observable<pacienteItemsDto> {
    return this.http.post<pacienteItemsDto>(this.url, body);
  }
  editar(guid: string, body: updatePacienteRequest): Observable<pacienteItemsDto> {
    return this.http.put<pacienteItemsDto>(`${this.url}/${guid}`, body);
  }
  eliminar(guid: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${guid}`);
  }
}
