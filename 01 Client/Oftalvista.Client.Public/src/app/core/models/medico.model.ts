import { paginatedItemsRequest } from './pagination.model';
export interface medicoItemsDto {
  rowNum?: number;
  idMedico: number;
  guid?: string;
  idUsuario: number;
  idEspecialidadMedica: number;
  cmp: string;
  perfilProfesional: string;
  idTblEstadoVigencia: number;
  rowCount?: number;
}
export interface medicoBusquedaRequest {
  idUsuario?: string;
  idEspecialidadMedica?: string;
  cmp?: string;
  fechaRegistroDesde?: string;
  fechaRegistroHasta?: string;
}
export interface createMedicoRequest {
  idUsuario: number;
  idEspecialidadMedica: number;
  cmp: string;
  perfilProfesional: string;
  idTblEstadoVigencia: number;
}
export interface updateMedicoRequest {
  guidMedico: string;
  idUsuario: number;
  idEspecialidadMedica: number;
  cmp: string;
  perfilProfesional: string;
  idTblEstadoVigencia: number;
}
export type medicoListRequest = paginatedItemsRequest<medicoBusquedaRequest>;
