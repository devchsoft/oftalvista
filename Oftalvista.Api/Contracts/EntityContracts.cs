namespace Oftalvista.Api.Contracts;

public abstract class PaginationQuery
{
    public int PageSize { get; set; } = 10;
    public int Skip { get; set; }
    public string? SortField { get; set; }
    public string? SortDir { get; set; }
}

public class UsuarioItemsDto
{
    public int? RowNum { get; set; }
    public int IdUsuario { get; set; }
    public string? Guid { get; set; }
    public int IdTipoUsuario { get; set; }
    public int? IdTipoDocumento { get; set; }
    public string? NumeroDocumento { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public int IdTblEstadoVigencia { get; set; }
    public int? RowCount { get; set; }
}

public class UsuarioListQuery : PaginationQuery
{
    public string? IdTipoUsuario { get; set; }
    public string? IdTipoDocumento { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? FechaRegistroDesde { get; set; }
    public string? FechaRegistroHasta { get; set; }
}

public class CreateUsuarioRequest
{
    public int IdTipoUsuario { get; set; }
    public int? IdTipoDocumento { get; set; }
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string ClaveHash { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}

public class UpdateUsuarioRequest
{
    public string? GuidUsuario { get; set; }
    public int IdTipoUsuario { get; set; }
    public int? IdTipoDocumento { get; set; }
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}

public class EspecialidadMedicaItemsDto
{
    public int? RowNum { get; set; }
    public int IdEspecialidadMedica { get; set; }
    public string? Guid { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int IdTblEstadoVigencia { get; set; }
    public int? RowCount { get; set; }
}

public class EspecialidadMedicaListQuery : PaginationQuery
{
    public string? Codigo { get; set; }
    public string? FechaRegistroDesde { get; set; }
    public string? FechaRegistroHasta { get; set; }
}

public class CreateEspecialidadMedicaRequest
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}

public class UpdateEspecialidadMedicaRequest
{
    public string? GuidEspecialidadMedica { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}

public class MedicoItemsDto
{
    public int? RowNum { get; set; }
    public int IdMedico { get; set; }
    public string? Guid { get; set; }
    public int IdUsuario { get; set; }
    public int IdEspecialidadMedica { get; set; }
    public string? Cmp { get; set; }
    public string? PerfilProfesional { get; set; }
    public int IdTblEstadoVigencia { get; set; }
    public int? RowCount { get; set; }
}

public class MedicoListQuery : PaginationQuery
{
    public string? IdUsuario { get; set; }
    public string? IdEspecialidadMedica { get; set; }
    public string? Cmp { get; set; }
    public string? FechaRegistroDesde { get; set; }
    public string? FechaRegistroHasta { get; set; }
}

public class CreateMedicoRequest
{
    public int IdUsuario { get; set; }
    public int IdEspecialidadMedica { get; set; }
    public string Cmp { get; set; } = string.Empty;
    public string? PerfilProfesional { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}

public class UpdateMedicoRequest
{
    public string? GuidMedico { get; set; }
    public int IdUsuario { get; set; }
    public int IdEspecialidadMedica { get; set; }
    public string Cmp { get; set; } = string.Empty;
    public string? PerfilProfesional { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}

public class PacienteItemsDto
{
    public int? RowNum { get; set; }
    public int IdPaciente { get; set; }
    public string? Guid { get; set; }
    public int IdUsuario { get; set; }
    public string? FechaNacimiento { get; set; }
    public string? Sexo { get; set; }
    public string? Direccion { get; set; }
    public string? ContactoEmergencia { get; set; }
    public string? TelefonoEmergencia { get; set; }
    public int IdTblEstadoVigencia { get; set; }
    public int? RowCount { get; set; }
}

public class PacienteListQuery : PaginationQuery
{
    public string? IdUsuario { get; set; }
    public string? FechaRegistroDesde { get; set; }
    public string? FechaRegistroHasta { get; set; }
}

public class CreatePacienteRequest
{
    public int IdUsuario { get; set; }
    public string? FechaNacimiento { get; set; }
    public string Sexo { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? ContactoEmergencia { get; set; }
    public string? TelefonoEmergencia { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}

public class UpdatePacienteRequest
{
    public string? GuidPaciente { get; set; }
    public int IdUsuario { get; set; }
    public string? FechaNacimiento { get; set; }
    public string Sexo { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? ContactoEmergencia { get; set; }
    public string? TelefonoEmergencia { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}

public class AgendaMedicaItemsDto
{
    public int? RowNum { get; set; }
    public int IdAgendaMedica { get; set; }
    public string? Guid { get; set; }
    public int IdMedico { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public string HoraInicio { get; set; } = string.Empty;
    public string HoraFin { get; set; } = string.Empty;
    public bool EsDisponible { get; set; }
    public string? Observacion { get; set; }
    public int IdTblEstadoVigencia { get; set; }
    public int? RowCount { get; set; }
}

public class AgendaMedicaListQuery : PaginationQuery
{
    public string? IdMedico { get; set; }
    public string? Fecha { get; set; }
    public string? FechaRegistroDesde { get; set; }
    public string? FechaRegistroHasta { get; set; }
}

public class CreateAgendaMedicaRequest
{
    public int IdMedico { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public string HoraInicio { get; set; } = string.Empty;
    public string HoraFin { get; set; } = string.Empty;
    public bool EsDisponible { get; set; }
    public string? Observacion { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}

public class UpdateAgendaMedicaRequest
{
    public string? GuidAgendaMedica { get; set; }
    public int IdMedico { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public string HoraInicio { get; set; } = string.Empty;
    public string HoraFin { get; set; } = string.Empty;
    public bool EsDisponible { get; set; }
    public string? Observacion { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}

public class CitaItemsDto
{
    public int? RowNum { get; set; }
    public int IdCita { get; set; }
    public string? Guid { get; set; }
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public int IdAgendaMedica { get; set; }
    public int IdEstadoCita { get; set; }
    public int IdModalidadCita { get; set; }
    public string? Motivo { get; set; }
    public string FechaCita { get; set; } = string.Empty;
    public string HoraCita { get; set; } = string.Empty;
    public string? Observacion { get; set; }
    public int IdTblEstadoVigencia { get; set; }
    public int? RowCount { get; set; }
}

public class CitaListQuery : PaginationQuery
{
    public string? IdPaciente { get; set; }
    public string? IdMedico { get; set; }
    public string? FechaRegistroDesde { get; set; }
    public string? FechaRegistroHasta { get; set; }
}

public class CreateCitaRequest
{
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public int IdAgendaMedica { get; set; }
    public int IdEstadoCita { get; set; }
    public int IdModalidadCita { get; set; }
    public string? Motivo { get; set; }
    public string FechaCita { get; set; } = string.Empty;
    public string HoraCita { get; set; } = string.Empty;
    public string? Observacion { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}

public class UpdateCitaRequest
{
    public string? GuidCita { get; set; }
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public int IdAgendaMedica { get; set; }
    public int IdEstadoCita { get; set; }
    public int IdModalidadCita { get; set; }
    public string? Motivo { get; set; }
    public string FechaCita { get; set; } = string.Empty;
    public string HoraCita { get; set; } = string.Empty;
    public string? Observacion { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}

public class HistorialCitaItemsDto
{
    public int? RowNum { get; set; }
    public int IdHistorialCita { get; set; }
    public string? Guid { get; set; }
    public int IdCita { get; set; }
    public int IdEstadoCita { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string FechaEvento { get; set; } = string.Empty;
    public int IdTblEstadoVigencia { get; set; }
    public int? RowCount { get; set; }
}

public class HistorialCitaListQuery : PaginationQuery
{
    public string? IdCita { get; set; }
    public string? IdEstadoCita { get; set; }
    public string? FechaRegistroDesde { get; set; }
    public string? FechaRegistroHasta { get; set; }
}

public class CreateHistorialCitaRequest
{
    public int IdCita { get; set; }
    public int IdEstadoCita { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string FechaEvento { get; set; } = string.Empty;
    public int IdTblEstadoVigencia { get; set; }
}

public class UpdateHistorialCitaRequest
{
    public string? GuidHistorialCita { get; set; }
    public int IdCita { get; set; }
    public int IdEstadoCita { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string FechaEvento { get; set; } = string.Empty;
    public int IdTblEstadoVigencia { get; set; }
}

public class PagoCitaItemsDto
{
    public int? RowNum { get; set; }
    public int IdPagoCita { get; set; }
    public string? Guid { get; set; }
    public int IdCita { get; set; }
    public int IdMetodoPago { get; set; }
    public int IdEstadoPago { get; set; }
    public decimal Monto { get; set; }
    public string? FechaPago { get; set; }
    public string? NumeroOperacion { get; set; }
    public string? Comprobante { get; set; }
    public int IdTblEstadoVigencia { get; set; }
    public int? RowCount { get; set; }
}

public class PagoCitaListQuery : PaginationQuery
{
    public string? IdCita { get; set; }
    public string? FechaRegistroDesde { get; set; }
    public string? FechaRegistroHasta { get; set; }
}

public class CreatePagoCitaRequest
{
    public int IdCita { get; set; }
    public int IdMetodoPago { get; set; }
    public int IdEstadoPago { get; set; }
    public decimal Monto { get; set; }
    public string? FechaPago { get; set; }
    public string? NumeroOperacion { get; set; }
    public string? Comprobante { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}

public class UpdatePagoCitaRequest
{
    public string? GuidPagoCita { get; set; }
    public int IdCita { get; set; }
    public int IdMetodoPago { get; set; }
    public int IdEstadoPago { get; set; }
    public decimal Monto { get; set; }
    public string? FechaPago { get; set; }
    public string? NumeroOperacion { get; set; }
    public string? Comprobante { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}

public class RecordatorioCitaItemsDto
{
    public int? RowNum { get; set; }
    public int IdRecordatorioCita { get; set; }
    public string? Guid { get; set; }
    public int IdCita { get; set; }
    public int IdEstadoRecordatorio { get; set; }
    public string FechaProgramada { get; set; } = string.Empty;
    public string? FechaEnvio { get; set; }
    public string? Mensaje { get; set; }
    public int IdTblEstadoVigencia { get; set; }
    public int? RowCount { get; set; }
}

public class RecordatorioCitaListQuery : PaginationQuery
{
    public string? IdCita { get; set; }
    public string? FechaRegistroDesde { get; set; }
    public string? FechaRegistroHasta { get; set; }
}

public class CreateRecordatorioCitaRequest
{
    public int IdCita { get; set; }
    public int IdEstadoRecordatorio { get; set; }
    public string FechaProgramada { get; set; } = string.Empty;
    public string? FechaEnvio { get; set; }
    public string? Mensaje { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}

public class UpdateRecordatorioCitaRequest
{
    public string? GuidRecordatorioCita { get; set; }
    public int IdCita { get; set; }
    public int IdEstadoRecordatorio { get; set; }
    public string FechaProgramada { get; set; } = string.Empty;
    public string? FechaEnvio { get; set; }
    public string? Mensaje { get; set; }
    public int IdTblEstadoVigencia { get; set; }
}
