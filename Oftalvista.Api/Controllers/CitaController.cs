using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oftalvista.Api.Contracts;
using Oftalvista.Api.Data;
using Oftalvista.Api.Domain;
using Oftalvista.Api.Helpers;

namespace Oftalvista.Api.Controllers;

[ApiController]
[Route("api/v1/cita")]
public class CitaController : ControllerBase
{
    private readonly OftalvistaDbContext _dbContext;

    public CitaController(OftalvistaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedItemsResponse<CitaItemsDto>>> Listar(
        [FromQuery] CitaListQuery query
    )
    {
        var pageSize = ApiResponseFactory.NormalizePageSize(query.PageSize);
        var skip = ApiResponseFactory.NormalizeSkip(query.Skip);
        var from = Formatters.ParseDateRangeStart(query.FechaRegistroDesde);
        var to = Formatters.ParseDateRangeEnd(query.FechaRegistroHasta);
        var baseQuery = _dbContext.Citas.AsNoTracking().AsQueryable();

        if (int.TryParse(query.IdPaciente, out var idPaciente))
        {
            baseQuery = baseQuery.Where(item => item.IdPaciente == idPaciente);
        }

        if (int.TryParse(query.IdMedico, out var idMedico))
        {
            baseQuery = baseQuery.Where(item => item.IdMedico == idMedico);
        }

        if (from.HasValue)
        {
            baseQuery = baseQuery.Where(item => item.FechaCreacion >= from.Value);
        }

        if (to.HasValue)
        {
            baseQuery = baseQuery.Where(item => item.FechaCreacion < to.Value);
        }

        baseQuery = ApiResponseFactory.IsDescending(query.SortDir)
            ? baseQuery.OrderByDescending(item => item.IdCita)
            : baseQuery.OrderBy(item => item.IdCita);

        var count = await baseQuery.CountAsync();
        var data = await baseQuery
            .Skip(skip)
            .Take(pageSize)
            .Select(item => new CitaItemsDto
            {
                IdCita = item.IdCita,
                Guid = item.Guid.ToString(),
                IdPaciente = item.IdPaciente,
                IdMedico = item.IdMedico,
                IdAgendaMedica = item.IdAgendaMedica,
                IdEstadoCita = item.IdEstadoCita,
                IdModalidadCita = item.IdModalidadCita,
                Motivo = item.Motivo,
                FechaCita = Formatters.ToDateText(item.FechaCita),
                HoraCita = Formatters.ToTimeText(item.HoraCita),
                Observacion = item.Observacion,
                IdTblEstadoVigencia = item.IdTblEstadoVigencia,
            })
            .ToListAsync();

        for (var index = 0; index < data.Count; index++)
        {
            data[index].RowNum = skip + index + 1;
            data[index].RowCount = count;
        }

        return Ok(ApiResponseFactory.Page(data, count, pageSize, skip));
    }

    [HttpGet("{guid}")]
    public async Task<ActionResult<CitaItemsDto>> Obtener(string guid)
    {
        var entity = await _dbContext.Citas.AsNoTracking().FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Cita no encontrada." });
        }

        return Ok(Map(entity));
    }

    [HttpPost]
    public async Task<ActionResult<CitaItemsDto>> Crear([FromBody] CreateCitaRequest request)
    {
        var validation = await ValidateCitaReferencesAsync(
            request.IdPaciente,
            request.IdMedico,
            request.IdAgendaMedica,
            null
        );
        if (validation is not null)
        {
            return validation;
        }

        var entity = new Cita
        {
            IdPaciente = request.IdPaciente,
            IdMedico = request.IdMedico,
            IdAgendaMedica = request.IdAgendaMedica,
            IdEstadoCita = request.IdEstadoCita,
            IdModalidadCita = request.IdModalidadCita,
            Motivo = request.Motivo,
            FechaCita = Formatters.ParseDateOnly(request.FechaCita),
            HoraCita = Formatters.ParseTimeOnly(request.HoraCita),
            Observacion = request.Observacion,
            IdTblEstadoVigencia = request.IdTblEstadoVigencia,
            EsEliminado = false,
        };

        _dbContext.Citas.Add(entity);
        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpPut("{guid}")]
    public async Task<ActionResult<CitaItemsDto>> Editar(string guid, [FromBody] UpdateCitaRequest request)
    {
        var entity = await _dbContext.Citas.FirstOrDefaultAsync(item => item.Guid.ToString() == guid);
        if (entity is null)
        {
            return NotFound(new { message = "Cita no encontrada." });
        }

        var validation = await ValidateCitaReferencesAsync(
            request.IdPaciente,
            request.IdMedico,
            request.IdAgendaMedica,
            entity.IdCita
        );
        if (validation is not null)
        {
            return validation;
        }

        entity.IdPaciente = request.IdPaciente;
        entity.IdMedico = request.IdMedico;
        entity.IdAgendaMedica = request.IdAgendaMedica;
        entity.IdEstadoCita = request.IdEstadoCita;
        entity.IdModalidadCita = request.IdModalidadCita;
        entity.Motivo = request.Motivo;
        entity.FechaCita = Formatters.ParseDateOnly(request.FechaCita);
        entity.HoraCita = Formatters.ParseTimeOnly(request.HoraCita);
        entity.Observacion = request.Observacion;
        entity.IdTblEstadoVigencia = request.IdTblEstadoVigencia;

        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpDelete("{guid}")]
    public async Task<IActionResult> Eliminar(string guid)
    {
        var entity = await _dbContext.Citas.FirstOrDefaultAsync(item => item.Guid.ToString() == guid);
        if (entity is null)
        {
            return NotFound(new { message = "Cita no encontrada." });
        }

        entity.EsEliminado = true;
        entity.IdTblEstadoVigencia = 2;
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private async Task<ActionResult?> ValidateCitaReferencesAsync(
        int idPaciente,
        int idMedico,
        int idAgendaMedica,
        int? currentId
    )
    {
        var pacienteExists = await _dbContext.Pacientes.AnyAsync(item => item.IdPaciente == idPaciente);
        var medicoExists = await _dbContext.Medicos.AnyAsync(item => item.IdMedico == idMedico);
        var agenda = await _dbContext.AgendasMedicas.FirstOrDefaultAsync(item =>
            item.IdAgendaMedica == idAgendaMedica && item.IdMedico == idMedico
        );

        if (!pacienteExists || !medicoExists || agenda is null)
        {
            return BadRequest(new { message = "Paciente, medico o agenda no valida." });
        }

        if (agenda.IdTblEstadoVigencia != 1 || !agenda.EsDisponible)
        {
            return BadRequest(new { message = "La agenda seleccionada no esta disponible." });
        }

        var agendaOcupada = await _dbContext.Citas.AnyAsync(item =>
            item.IdAgendaMedica == idAgendaMedica
            && item.IdTblEstadoVigencia == 1
            && item.IdEstadoCita != 4
            && (!currentId.HasValue || item.IdCita != currentId.Value)
        );

        if (agendaOcupada)
        {
            return BadRequest(new { message = "La agenda seleccionada ya tiene una cita activa." });
        }

        return null;
    }

    private static CitaItemsDto Map(Cita item) =>
        new()
        {
            IdCita = item.IdCita,
            Guid = item.Guid.ToString(),
            IdPaciente = item.IdPaciente,
            IdMedico = item.IdMedico,
            IdAgendaMedica = item.IdAgendaMedica,
            IdEstadoCita = item.IdEstadoCita,
            IdModalidadCita = item.IdModalidadCita,
            Motivo = item.Motivo,
            FechaCita = Formatters.ToDateText(item.FechaCita),
            HoraCita = Formatters.ToTimeText(item.HoraCita),
            Observacion = item.Observacion,
            IdTblEstadoVigencia = item.IdTblEstadoVigencia,
        };
}
