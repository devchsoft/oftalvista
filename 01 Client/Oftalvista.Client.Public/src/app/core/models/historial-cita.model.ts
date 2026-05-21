import { paginatedItemsRequest } from './pagination.model';
export interface historialCitaItemsDto {
  rowNum?: number;
  idHistorialCita: number;
  guid?: string;
  idCita: number;
  idEstadoCita: number;
  descripcion: string;
  fechaEvento: string;
  idTblEstadoVigencia: number;
  rowCount?: number;
}
export interface historialCitaBusquedaRequest {
  idCita?: string;
  idEstadoCita?: string;
  fechaRegistroDesde?: string;
  fechaRegistroHasta?: string;
}
export interface createHistorialCitaRequest {
  idCita: number;
  idEstadoCita: number;
  descripcion: string;
  fechaEvento: string;
  idTblEstadoVigencia: number;
}
export interface updateHistorialCitaRequest {
  guidHistorialCita: string;
  idCita: number;
  idEstadoCita: number;
  descripcion: string;
  fechaEvento: string;
  idTblEstadoVigencia: number;
}
export type historialCitaListRequest = paginatedItemsRequest<historialCitaBusquedaRequest>;
