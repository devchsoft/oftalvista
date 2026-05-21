import { paginatedItemsRequest } from './pagination.model';
export interface especialidadMedicaItemsDto {
  rowNum?: number;
  idEspecialidadMedica: number;
  guid?: string;
  codigo: string;
  nombre: string;
  descripcion: string;
  idTblEstadoVigencia: number;
  rowCount?: number;
}
export interface especialidadMedicaBusquedaRequest {
  codigo?: string;
  fechaRegistroDesde?: string;
  fechaRegistroHasta?: string;
}
export interface createEspecialidadMedicaRequest {
  codigo: string;
  nombre: string;
  descripcion: string;
  idTblEstadoVigencia: number;
}
export interface updateEspecialidadMedicaRequest {
  guidEspecialidadMedica: string;
  codigo: string;
  nombre: string;
  descripcion: string;
  idTblEstadoVigencia: number;
}
export type especialidadMedicaListRequest =
  paginatedItemsRequest<especialidadMedicaBusquedaRequest>;
