using Microsoft.EntityFrameworkCore;
using Oftalvista.Api.Domain;

namespace Oftalvista.Api.Data;

public class OftalvistaDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OftalvistaDbContext(
        DbContextOptions<OftalvistaDbContext> options,
        IHttpContextAccessor httpContextAccessor
    )
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<EspecialidadMedica> EspecialidadesMedicas => Set<EspecialidadMedica>();
    public DbSet<Medico> Medicos => Set<Medico>();
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<AgendaMedica> AgendasMedicas => Set<AgendaMedica>();
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<HistorialCita> HistorialCitas => Set<HistorialCita>();
    public DbSet<PagoCita> PagosCita => Set<PagoCita>();
    public DbSet<RecordatorioCita> RecordatoriosCita => Set<RecordatorioCita>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUsuario(modelBuilder.Entity<Usuario>());
        ConfigureEspecialidad(modelBuilder.Entity<EspecialidadMedica>());
        ConfigureMedico(modelBuilder.Entity<Medico>());
        ConfigurePaciente(modelBuilder.Entity<Paciente>());
        ConfigureAgenda(modelBuilder.Entity<AgendaMedica>());
        ConfigureCita(modelBuilder.Entity<Cita>());
        ConfigureHistorial(modelBuilder.Entity<HistorialCita>());
        ConfigurePago(modelBuilder.Entity<PagoCita>());
        ConfigureRecordatorio(modelBuilder.Entity<RecordatorioCita>());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInformation()
    {
        var userName =
            _httpContextAccessor.HttpContext?.Request.Headers["X-User-Email"].FirstOrDefault()
            ?? _httpContextAccessor.HttpContext?.User?.Identity?.Name
            ?? "oftalvista-api";
        var ipAddress =
            _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.Guid == Guid.Empty)
                {
                    entry.Entity.Guid = Guid.NewGuid();
                }

                entry.Entity.UsuarioCreacion = userName;
                entry.Entity.FechaCreacion = now;
                entry.Entity.IpCreacion = ipAddress;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UsuarioModificacion = userName;
                entry.Entity.FechaModificacion = now;
                entry.Entity.IpModificacion = ipAddress;
            }
        }
    }

    private static void ConfigureUsuario(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Usuario> entity)
    {
        entity.ToTable("usuario", "maestro");
        entity.HasKey(item => item.IdUsuario);
        entity.HasQueryFilter(item => !item.EsEliminado);
        entity.Property(item => item.IdUsuario).HasColumnName("ID_USUARIO").ValueGeneratedOnAdd();
        entity.Property(item => item.IdTipoUsuario).HasColumnName("ID_TIPO_USUARIO");
        entity.Property(item => item.IdTipoDocumento).HasColumnName("ID_TIPO_DOCUMENTO");
        entity.Property(item => item.NumeroDocumento).HasColumnName("NUMERO_DOCUMENTO");
        entity.Property(item => item.Nombres).HasColumnName("NOMBRES");
        entity.Property(item => item.Apellidos).HasColumnName("APELLIDOS");
        entity.Property(item => item.Correo).HasColumnName("CORREO");
        entity.Property(item => item.ClaveHash).HasColumnName("CLAVE_HASH");
        entity.Property(item => item.Telefono).HasColumnName("TELEFONO");
        ConfigureAudit(entity);
    }

    private static void ConfigureEspecialidad(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<EspecialidadMedica> entity)
    {
        entity.ToTable("especialidad_medica", "maestro");
        entity.HasKey(item => item.IdEspecialidadMedica);
        entity.HasQueryFilter(item => !item.EsEliminado);
        entity.Property(item => item.IdEspecialidadMedica).HasColumnName("ID_ESPECIALIDAD_MEDICA").ValueGeneratedOnAdd();
        entity.Property(item => item.Codigo).HasColumnName("CODIGO");
        entity.Property(item => item.Nombre).HasColumnName("NOMBRE");
        entity.Property(item => item.Descripcion).HasColumnName("DESCRIPCION");
        ConfigureAudit(entity);
    }

    private static void ConfigureMedico(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Medico> entity)
    {
        entity.ToTable("medico", "maestro");
        entity.HasKey(item => item.IdMedico);
        entity.HasQueryFilter(item => !item.EsEliminado);
        entity.Property(item => item.IdMedico).HasColumnName("ID_MEDICO").ValueGeneratedOnAdd();
        entity.Property(item => item.IdUsuario).HasColumnName("ID_USUARIO");
        entity.Property(item => item.IdEspecialidadMedica).HasColumnName("ID_ESPECIALIDAD_MEDICA");
        entity.Property(item => item.Cmp).HasColumnName("CMP");
        entity.Property(item => item.PerfilProfesional).HasColumnName("PERFIL_PROFESIONAL");
        ConfigureAudit(entity);
    }

    private static void ConfigurePaciente(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Paciente> entity)
    {
        entity.ToTable("paciente", "maestro");
        entity.HasKey(item => item.IdPaciente);
        entity.HasQueryFilter(item => !item.EsEliminado);
        entity.Property(item => item.IdPaciente).HasColumnName("ID_PACIENTE").ValueGeneratedOnAdd();
        entity.Property(item => item.IdUsuario).HasColumnName("ID_USUARIO");
        entity.Property(item => item.FechaNacimiento).HasColumnName("FECHA_NACIMIENTO").HasColumnType("date");
        entity.Property(item => item.Sexo).HasColumnName("SEXO");
        entity.Property(item => item.Direccion).HasColumnName("DIRECCION");
        entity.Property(item => item.ContactoEmergencia).HasColumnName("CONTACTO_EMERGENCIA");
        entity.Property(item => item.TelefonoEmergencia).HasColumnName("TELEFONO_EMERGENCIA");
        ConfigureAudit(entity);
    }

    private static void ConfigureAgenda(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<AgendaMedica> entity)
    {
        entity.ToTable("agenda_medica", "transaccional");
        entity.HasKey(item => item.IdAgendaMedica);
        entity.HasQueryFilter(item => !item.EsEliminado);
        entity.Property(item => item.IdAgendaMedica).HasColumnName("ID_AGENDA_MEDICA").ValueGeneratedOnAdd();
        entity.Property(item => item.IdMedico).HasColumnName("ID_MEDICO");
        entity.Property(item => item.Fecha).HasColumnName("FECHA").HasColumnType("date");
        entity.Property(item => item.HoraInicio).HasColumnName("HORA_INICIO").HasColumnType("time");
        entity.Property(item => item.HoraFin).HasColumnName("HORA_FIN").HasColumnType("time");
        entity.Property(item => item.EsDisponible).HasColumnName("ES_DISPONIBLE");
        entity.Property(item => item.Observacion).HasColumnName("OBSERVACION");
        ConfigureAudit(entity);
    }

    private static void ConfigureCita(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Cita> entity)
    {
        entity.ToTable("cita", "transaccional");
        entity.HasKey(item => item.IdCita);
        entity.HasQueryFilter(item => !item.EsEliminado);
        entity.Property(item => item.IdCita).HasColumnName("ID_CITA").ValueGeneratedOnAdd();
        entity.Property(item => item.IdPaciente).HasColumnName("ID_PACIENTE");
        entity.Property(item => item.IdMedico).HasColumnName("ID_MEDICO");
        entity.Property(item => item.IdAgendaMedica).HasColumnName("ID_AGENDA_MEDICA");
        entity.Property(item => item.IdEstadoCita).HasColumnName("ID_ESTADO_CITA");
        entity.Property(item => item.IdModalidadCita).HasColumnName("ID_MODALIDAD_CITA");
        entity.Property(item => item.Motivo).HasColumnName("MOTIVO");
        entity.Property(item => item.FechaCita).HasColumnName("FECHA_CITA").HasColumnType("date");
        entity.Property(item => item.HoraCita).HasColumnName("HORA_CITA").HasColumnType("time");
        entity.Property(item => item.Observacion).HasColumnName("OBSERVACION");
        ConfigureAudit(entity);
    }

    private static void ConfigureHistorial(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<HistorialCita> entity)
    {
        entity.ToTable("historial_cita", "transaccional");
        entity.HasKey(item => item.IdHistorialCita);
        entity.HasQueryFilter(item => !item.EsEliminado);
        entity.Property(item => item.IdHistorialCita).HasColumnName("ID_HISTORIAL_CITA").ValueGeneratedOnAdd();
        entity.Property(item => item.IdCita).HasColumnName("ID_CITA");
        entity.Property(item => item.IdEstadoCita).HasColumnName("ID_ESTADO_CITA");
        entity.Property(item => item.Descripcion).HasColumnName("DESCRIPCION");
        entity.Property(item => item.FechaEvento).HasColumnName("FECHA_EVENTO");
        ConfigureAudit(entity);
    }

    private static void ConfigurePago(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<PagoCita> entity)
    {
        entity.ToTable("pago_cita", "transaccional");
        entity.HasKey(item => item.IdPagoCita);
        entity.HasQueryFilter(item => !item.EsEliminado);
        entity.Property(item => item.IdPagoCita).HasColumnName("ID_PAGO_CITA").ValueGeneratedOnAdd();
        entity.Property(item => item.IdCita).HasColumnName("ID_CITA");
        entity.Property(item => item.IdMetodoPago).HasColumnName("ID_METODO_PAGO");
        entity.Property(item => item.IdEstadoPago).HasColumnName("ID_ESTADO_PAGO");
        entity.Property(item => item.Monto).HasColumnName("MONTO").HasColumnType("decimal(18,2)");
        entity.Property(item => item.FechaPago).HasColumnName("FECHA_PAGO");
        entity.Property(item => item.NumeroOperacion).HasColumnName("NUMERO_OPERACION");
        entity.Property(item => item.Comprobante).HasColumnName("COMPROBANTE");
        ConfigureAudit(entity);
    }

    private static void ConfigureRecordatorio(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<RecordatorioCita> entity)
    {
        entity.ToTable("recordatorio_cita", "transaccional");
        entity.HasKey(item => item.IdRecordatorioCita);
        entity.HasQueryFilter(item => !item.EsEliminado);
        entity.Property(item => item.IdRecordatorioCita).HasColumnName("ID_RECORDATORIO_CITA").ValueGeneratedOnAdd();
        entity.Property(item => item.IdCita).HasColumnName("ID_CITA");
        entity.Property(item => item.IdEstadoRecordatorio).HasColumnName("ID_ESTADO_RECORDATORIO");
        entity.Property(item => item.FechaProgramada).HasColumnName("FECHA_PROGRAMADA");
        entity.Property(item => item.FechaEnvio).HasColumnName("FECHA_ENVIO");
        entity.Property(item => item.Mensaje).HasColumnName("MENSAJE");
        ConfigureAudit(entity);
    }

    private static void ConfigureAudit<TEntity>(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> entity
    )
        where TEntity : AuditableEntity
    {
        entity.Property(item => item.IdTblEstadoVigencia).HasColumnName("ID_TBL_ESTADO_VIGENCIA");
        entity.Property(item => item.Guid).HasColumnName("GUID");
        entity.Property(item => item.EsEliminado).HasColumnName("ES_ELIMINADO");
        entity.Property(item => item.UsuarioCreacion).HasColumnName("USUARIO_CREACION");
        entity.Property(item => item.FechaCreacion).HasColumnName("FECHA_CREACION");
        entity.Property(item => item.IpCreacion).HasColumnName("IP_CREACION");
        entity.Property(item => item.UsuarioModificacion).HasColumnName("USUARIO_MODIFICACION");
        entity.Property(item => item.FechaModificacion).HasColumnName("FECHA_MODIFICACION");
        entity.Property(item => item.IpModificacion).HasColumnName("IP_MODIFICACION");
    }
}
