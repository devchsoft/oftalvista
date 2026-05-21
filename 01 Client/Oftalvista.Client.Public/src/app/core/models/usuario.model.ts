import { paginatedItemsRequest } from './pagination.model';
export interface usuarioItemsDto {
  rowNum?: number;
  idUsuario: number;
  guid?: string;
  idTipoUsuario: number;
  idTipoDocumento?: number;
  numeroDocumento: string;
  nombres: string;
  apellidos: string;
  correo: string;
  telefono: string;
  idTblEstadoVigencia: number;
  rowCount?: number;
}
export interface usuarioBusquedaRequest {
  idTipoUsuario?: string;
  idTipoDocumento?: string;
  numeroDocumento?: string;
  fechaRegistroDesde?: string;
  fechaRegistroHasta?: string;
}
export interface createUsuarioRequest {
  idTipoUsuario: number;
  idTipoDocumento?: number;
  numeroDocumento: string;
  nombres: string;
  apellidos: string;
  correo: string;
  claveHash: string;
  telefono: string;
  idTblEstadoVigencia: number;
}
export interface updateUsuarioRequest {
  guidUsuario: string;
  idTipoUsuario: number;
  idTipoDocumento?: number;
  numeroDocumento: string;
  nombres: string;
  apellidos: string;
  correo: string;
  telefono: string;
  idTblEstadoVigencia: number;
}
export type usuarioListRequest = paginatedItemsRequest<usuarioBusquedaRequest>;
