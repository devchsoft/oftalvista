import { paginatedItemsRequest } from './pagination.model';
export interface agendaMedicaItemsDto {
  rowNum?: number;
  idAgendaMedica: number;
  guid?: string;
  idMedico: number;
  fecha: string;
  horaInicio: string;
  horaFin: string;
  esDisponible: boolean;
  observacion: string;
  idTblEstadoVigencia: number;
  rowCount?: number;
}
export interface agendaMedicaBusquedaRequest {
  idMedico?: string;
  fecha?: string;
  fechaRegistroDesde?: string;
  fechaRegistroHasta?: string;
}
export interface createAgendaMedicaRequest {
  idMedico: number;
  fecha: string;
  horaInicio: string;
  horaFin: string;
  esDisponible: boolean;
  observacion: string;
  idTblEstadoVigencia: number;
}
export interface updateAgendaMedicaRequest {
  guidAgendaMedica: string;
  idMedico: number;
  fecha: string;
  horaInicio: string;
  horaFin: string;
  esDisponible: boolean;
  observacion: string;
  idTblEstadoVigencia: number;
}
export type agendaMedicaListRequest = paginatedItemsRequest<agendaMedicaBusquedaRequest>;
