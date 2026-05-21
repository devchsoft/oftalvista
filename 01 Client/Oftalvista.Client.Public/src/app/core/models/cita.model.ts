import { paginatedItemsRequest } from './pagination.model';
export interface citaItemsDto {
  rowNum?: number;
  idCita: number;
  guid?: string;
  idPaciente: number;
  idMedico: number;
  idAgendaMedica: number;
  idEstadoCita: number;
  idModalidadCita: number;
  motivo: string;
  fechaCita: string;
  horaCita: string;
  observacion: string;
  idTblEstadoVigencia: number;
  rowCount?: number;
}
export interface citaBusquedaRequest {
  idPaciente?: string;
  idMedico?: string;
  fechaRegistroDesde?: string;
  fechaRegistroHasta?: string;
}
export interface createCitaRequest {
  idPaciente: number;
  idMedico: number;
  idAgendaMedica: number;
  idEstadoCita: number;
  idModalidadCita: number;
  motivo: string;
  fechaCita: string;
  horaCita: string;
  observacion: string;
  idTblEstadoVigencia: number;
}
export interface updateCitaRequest {
  guidCita: string;
  idPaciente: number;
  idMedico: number;
  idAgendaMedica: number;
  idEstadoCita: number;
  idModalidadCita: number;
  motivo: string;
  fechaCita: string;
  horaCita: string;
  observacion: string;
  idTblEstadoVigencia: number;
}
export type citaListRequest = paginatedItemsRequest<citaBusquedaRequest>;
