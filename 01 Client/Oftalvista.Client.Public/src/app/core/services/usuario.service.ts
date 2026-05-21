import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { paginatedItemsResponse } from '../models/pagination.model';
import {
  usuarioItemsDto,
  createUsuarioRequest,
  updateUsuarioRequest,
  usuarioListRequest,
} from '../models/usuario.model';
@Injectable({ providedIn: 'root' })
export class UsuarioService {
  private url = `${environment.apiUrl}/usuario`;
  constructor(private http: HttpClient) {}
  listar(req: usuarioListRequest): Observable<paginatedItemsResponse<usuarioItemsDto>> {
    let p = new HttpParams().set('pageSize', req.pageSize).set('skip', req.skip);
    if (req.sortField) p = p.set('sortField', req.sortField);
    if (req.sortDir) p = p.set('sortDir', req.sortDir);
    if (req.filter)
      Object.keys(req.filter).forEach((k) => {
        const v = (req.filter as any)[k];
        if (v != null && v !== '') p = p.set(k, v);
      });
    return this.http.get<paginatedItemsResponse<usuarioItemsDto>>(this.url, { params: p });
  }
  obtener(guid: string): Observable<usuarioItemsDto> {
    return this.http.get<usuarioItemsDto>(`${this.url}/${guid}`);
  }
  crear(body: createUsuarioRequest): Observable<usuarioItemsDto> {
    return this.http.post<usuarioItemsDto>(this.url, body);
  }
  editar(guid: string, body: updateUsuarioRequest): Observable<usuarioItemsDto> {
    return this.http.put<usuarioItemsDto>(`${this.url}/${guid}`, body);
  }
  eliminar(guid: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${guid}`);
  }
}
