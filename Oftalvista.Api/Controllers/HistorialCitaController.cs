using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oftalvista.Api.Contracts;
using Oftalvista.Api.Data;
using Oftalvista.Api.Domain;
using Oftalvista.Api.Helpers;

namespace Oftalvista.Api.Controllers;

[ApiController]
[Route("api/v1/historial-cita")]
public class HistorialCitaController : ControllerBase
{
    private readonly OftalvistaDbContext _dbContext;

    public HistorialCitaController(OftalvistaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedItemsResponse<HistorialCitaItemsDto>>> Listar(
        [FromQuery] HistorialCitaListQuery query
    )
    {
        var pageSize = ApiResponseFactory.NormalizePageSize(query.PageSize);
        var skip = ApiResponseFactory.NormalizeSkip(query.Skip);
        var from = Formatters.ParseDateRangeStart(query.FechaRegistroDesde);
        var to = Formatters.ParseDateRangeEnd(query.FechaRegistroHasta);
        var baseQuery = _dbContext.HistorialCitas.AsNoTracking().AsQueryable();

        if (int.TryParse(query.IdCita, out var idCita))
        {
            baseQuery = baseQuery.Where(item => item.IdCita == idCita);
        }

        if (int.TryParse(query.IdEstadoCita, out var idEstadoCita))
        {
            baseQuery = baseQuery.Where(item => item.IdEstadoCita == idEstadoCita);
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
            ? baseQuery.OrderByDescending(item => item.IdHistorialCita)
            : baseQuery.OrderBy(item => item.IdHistorialCita);

        var count = await baseQuery.CountAsync();
        var data = await baseQuery
            .Skip(skip)
            .Take(pageSize)
            .Select(item => new HistorialCitaItemsDto
            {
                IdHistorialCita = item.IdHistorialCita,
                Guid = item.Guid.ToString(),
                IdCita = item.IdCita,
                IdEstadoCita = item.IdEstadoCita,
                Descripcion = item.Descripcion,
                FechaEvento = Formatters.ToDateText(item.FechaEvento),
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
    public async Task<ActionResult<HistorialCitaItemsDto>> Obtener(string guid)
    {
        var entity = await _dbContext.HistorialCitas.AsNoTracking().FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Historial de cita no encontrado." });
        }

        return Ok(Map(entity));
    }

    [HttpPost]
    public async Task<ActionResult<HistorialCitaItemsDto>> Crear(
        [FromBody] CreateHistorialCitaRequest request
    )
    {
        var citaExists = await _dbContext.Citas.AnyAsync(item => item.IdCita == request.IdCita);
        if (!citaExists)
        {
            return BadRequest(new { message = "La cita asociada no existe." });
        }

        var entity = new HistorialCita
        {
            IdCita = request.IdCita,
            IdEstadoCita = request.IdEstadoCita,
            Descripcion = request.Descripcion,
            FechaEvento = Formatters.ParseDateTime(request.FechaEvento),
            IdTblEstadoVigencia = request.IdTblEstadoVigencia,
            EsEliminado = false,
        };

        _dbContext.HistorialCitas.Add(entity);
        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpPut("{guid}")]
    public async Task<ActionResult<HistorialCitaItemsDto>> Editar(
        string guid,
        [FromBody] UpdateHistorialCitaRequest request
    )
    {
        var entity = await _dbContext.HistorialCitas.FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Historial de cita no encontrado." });
        }

        entity.IdCita = request.IdCita;
        entity.IdEstadoCita = request.IdEstadoCita;
        entity.Descripcion = request.Descripcion;
        entity.FechaEvento = Formatters.ParseDateTime(request.FechaEvento);
        entity.IdTblEstadoVigencia = request.IdTblEstadoVigencia;

        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpDelete("{guid}")]
    public async Task<IActionResult> Eliminar(string guid)
    {
        var entity = await _dbContext.HistorialCitas.FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Historial de cita no encontrado." });
        }

        entity.EsEliminado = true;
        entity.IdTblEstadoVigencia = 2;
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static HistorialCitaItemsDto Map(HistorialCita item) =>
        new()
        {
            IdHistorialCita = item.IdHistorialCita,
            Guid = item.Guid.ToString(),
            IdCita = item.IdCita,
            IdEstadoCita = item.IdEstadoCita,
            Descripcion = item.Descripcion,
            FechaEvento = Formatters.ToDateText(item.FechaEvento),
            IdTblEstadoVigencia = item.IdTblEstadoVigencia,
        };
}
