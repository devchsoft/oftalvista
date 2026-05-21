import { paginatedItemsRequest } from './pagination.model';
export interface pacienteItemsDto {
  rowNum?: number;
  idPaciente: number;
  guid?: string;
  idUsuario: number;
  fechaNacimiento?: string;
  sexo: string;
  direccion: string;
  contactoEmergencia: string;
  telefonoEmergencia: string;
  idTblEstadoVigencia: number;
  rowCount?: number;
}
export interface pacienteBusquedaRequest {
  idUsuario?: string;
  fechaRegistroDesde?: string;
  fechaRegistroHasta?: string;
}
export interface createPacienteRequest {
  idUsuario: number;
  fechaNacimiento?: string;
  sexo: string;
  direccion: string;
  contactoEmergencia: string;
  telefonoEmergencia: string;
  idTblEstadoVigencia: number;
}
export interface updatePacienteRequest {
  guidPaciente: string;
  idUsuario: number;
  fechaNacimiento?: string;
  sexo: string;
  direccion: string;
  contactoEmergencia: string;
  telefonoEmergencia: string;
  idTblEstadoVigencia: number;
}
export type pacienteListRequest = paginatedItemsRequest<pacienteBusquedaRequest>;
