using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oftalvista.Api.Contracts;
using Oftalvista.Api.Data;
using Oftalvista.Api.Domain;
using Oftalvista.Api.Helpers;

namespace Oftalvista.Api.Controllers;

[ApiController]
[Route("api/v1/agenda-medica")]
public class AgendaMedicaController : ControllerBase
{
    private readonly OftalvistaDbContext _dbContext;

    public AgendaMedicaController(OftalvistaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedItemsResponse<AgendaMedicaItemsDto>>> Listar(
        [FromQuery] AgendaMedicaListQuery query
    )
    {
        var pageSize = ApiResponseFactory.NormalizePageSize(query.PageSize);
        var skip = ApiResponseFactory.NormalizeSkip(query.Skip);
        var from = Formatters.ParseDateRangeStart(query.FechaRegistroDesde);
        var to = Formatters.ParseDateRangeEnd(query.FechaRegistroHasta);
        var fecha = Formatters.ParseNullableDateOnly(query.Fecha);
        var baseQuery = _dbContext.AgendasMedicas.AsNoTracking().AsQueryable();

        if (int.TryParse(query.IdMedico, out var idMedico))
        {
            baseQuery = baseQuery.Where(item => item.IdMedico == idMedico);
        }

        if (fecha.HasValue)
        {
            baseQuery = baseQuery.Where(item => item.Fecha == fecha.Value);
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
            ? baseQuery.OrderByDescending(item => item.IdAgendaMedica)
            : baseQuery.OrderBy(item => item.IdAgendaMedica);

        var count = await baseQuery.CountAsync();
        var data = await baseQuery
            .Skip(skip)
            .Take(pageSize)
            .Select(item => new AgendaMedicaItemsDto
            {
                IdAgendaMedica = item.IdAgendaMedica,
                Guid = item.Guid.ToString(),
                IdMedico = item.IdMedico,
                Fecha = Formatters.ToDateText(item.Fecha),
                HoraInicio = Formatters.ToTimeText(item.HoraInicio),
                HoraFin = Formatters.ToTimeText(item.HoraFin),
                EsDisponible = item.EsDisponible,
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
    public async Task<ActionResult<AgendaMedicaItemsDto>> Obtener(string guid)
    {
        var entity = await _dbContext.AgendasMedicas.AsNoTracking().FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Agenda medica no encontrada." });
        }

        return Ok(Map(entity));
    }

    [HttpPost]
    public async Task<ActionResult<AgendaMedicaItemsDto>> Crear(
        [FromBody] CreateAgendaMedicaRequest request
    )
    {
        var medicoExists = await _dbContext.Medicos.AnyAsync(item => item.IdMedico == request.IdMedico);
        if (!medicoExists)
        {
            return BadRequest(new { message = "Medico no valido para la agenda." });
        }

        var entity = new AgendaMedica
        {
            IdMedico = request.IdMedico,
            Fecha = Formatters.ParseDateOnly(request.Fecha),
            HoraInicio = Formatters.ParseTimeOnly(request.HoraInicio),
            HoraFin = Formatters.ParseTimeOnly(request.HoraFin),
            EsDisponible = request.EsDisponible,
            Observacion = request.Observacion,
            IdTblEstadoVigencia = request.IdTblEstadoVigencia,
            EsEliminado = false,
        };

        _dbContext.AgendasMedicas.Add(entity);
        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpPut("{guid}")]
    public async Task<ActionResult<AgendaMedicaItemsDto>> Editar(
        string guid,
        [FromBody] UpdateAgendaMedicaRequest request
    )
    {
        var entity = await _dbContext.AgendasMedicas.FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Agenda medica no encontrada." });
        }

        entity.IdMedico = request.IdMedico;
        entity.Fecha = Formatters.ParseDateOnly(request.Fecha);
        entity.HoraInicio = Formatters.ParseTimeOnly(request.HoraInicio);
        entity.HoraFin = Formatters.ParseTimeOnly(request.HoraFin);
        entity.EsDisponible = request.EsDisponible;
        entity.Observacion = request.Observacion;
        entity.IdTblEstadoVigencia = request.IdTblEstadoVigencia;

        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpDelete("{guid}")]
    public async Task<IActionResult> Eliminar(string guid)
    {
        var entity = await _dbContext.AgendasMedicas.FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Agenda medica no encontrada." });
        }

        entity.EsEliminado = true;
        entity.IdTblEstadoVigencia = 2;
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static AgendaMedicaItemsDto Map(AgendaMedica item) =>
        new()
        {
            IdAgendaMedica = item.IdAgendaMedica,
            Guid = item.Guid.ToString(),
            IdMedico = item.IdMedico,
            Fecha = Formatters.ToDateText(item.Fecha),
            HoraInicio = Formatters.ToTimeText(item.HoraInicio),
            HoraFin = Formatters.ToTimeText(item.HoraFin),
            EsDisponible = item.EsDisponible,
            Observacion = item.Observacion,
            IdTblEstadoVigencia = item.IdTblEstadoVigencia,
        };
}
