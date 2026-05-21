namespace Oftalvista.Api.Domain;

public class Usuario : AuditableEntity
{
    public int IdUsuario { get; set; }
    public int IdTipoUsuario { get; set; }
    public int? IdTipoDocumento { get; set; }
    public string? NumeroDocumento { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string ClaveHash { get; set; } = string.Empty;
    public string? Telefono { get; set; }
}

public class EspecialidadMedica : AuditableEntity
{
    public int IdEspecialidadMedica { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class Medico : AuditableEntity
{
    public int IdMedico { get; set; }
    public int IdUsuario { get; set; }
    public int IdEspecialidadMedica { get; set; }
    public string? Cmp { get; set; }
    public string? PerfilProfesional { get; set; }
}

public class Paciente : AuditableEntity
{
    public int IdPaciente { get; set; }
    public int IdUsuario { get; set; }
    public DateOnly? FechaNacimiento { get; set; }
    public string? Sexo { get; set; }
    public string? Direccion { get; set; }
    public string? ContactoEmergencia { get; set; }
    public string? TelefonoEmergencia { get; set; }
}

public class AgendaMedica : AuditableEntity
{
    public int IdAgendaMedica { get; set; }
    public int IdMedico { get; set; }
    public DateOnly Fecha { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    public bool EsDisponible { get; set; }
    public string? Observacion { get; set; }
}

public class Cita : AuditableEntity
{
    public int IdCita { get; set; }
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public int IdAgendaMedica { get; set; }
    public int IdEstadoCita { get; set; }
    public int IdModalidadCita { get; set; }
    public string? Motivo { get; set; }
    public DateOnly FechaCita { get; set; }
    public TimeOnly HoraCita { get; set; }
    public string? Observacion { get; set; }
}

public class HistorialCita : AuditableEntity
{
    public int IdHistorialCita { get; set; }
    public int IdCita { get; set; }
    public int IdEstadoCita { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; }
}

public class PagoCita : AuditableEntity
{
    public int IdPagoCita { get; set; }
    public int IdCita { get; set; }
    public int IdMetodoPago { get; set; }
    public int IdEstadoPago { get; set; }
    public decimal Monto { get; set; }
    public DateTime? FechaPago { get; set; }
    public string? NumeroOperacion { get; set; }
    public string? Comprobante { get; set; }
}

public class RecordatorioCita : AuditableEntity
{
    public int IdRecordatorioCita { get; set; }
    public int IdCita { get; set; }
    public int IdEstadoRecordatorio { get; set; }
    public DateTime FechaProgramada { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public string? Mensaje { get; set; }
}
