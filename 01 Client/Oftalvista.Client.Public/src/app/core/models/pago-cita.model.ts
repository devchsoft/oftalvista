import { paginatedItemsRequest } from './pagination.model';
export interface pagoCitaItemsDto {
  rowNum?: number;
  idPagoCita: number;
  guid?: string;
  idCita: number;
  idMetodoPago: number;
  idEstadoPago: number;
  monto: number;
  fechaPago?: string;
  numeroOperacion: string;
  comprobante: string;
  idTblEstadoVigencia: number;
  rowCount?: number;
}
export interface pagoCitaBusquedaRequest {
  idCita?: string;
  fechaRegistroDesde?: string;
  fechaRegistroHasta?: string;
}
export interface createPagoCitaRequest {
  idCita: number;
  idMetodoPago: number;
  idEstadoPago: number;
  monto: number;
  fechaPago?: string;
  numeroOperacion: string;
  comprobante: string;
  idTblEstadoVigencia: number;
}
export interface updatePagoCitaRequest {
  guidPagoCita: string;
  idCita: number;
  idMetodoPago: number;
  idEstadoPago: number;
  monto: number;
  fechaPago?: string;
  numeroOperacion: string;
  comprobante: string;
  idTblEstadoVigencia: number;
}
export type pagoCitaListRequest = paginatedItemsRequest<pagoCitaBusquedaRequest>;
