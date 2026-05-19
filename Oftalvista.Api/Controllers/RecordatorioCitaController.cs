using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oftalvista.Api.Contracts;
using Oftalvista.Api.Data;
using Oftalvista.Api.Domain;
using Oftalvista.Api.Helpers;

namespace Oftalvista.Api.Controllers;

[ApiController]
[Route("api/v1/recordatorio-cita")]
public class RecordatorioCitaController : ControllerBase
{
    private readonly OftalvistaDbContext _dbContext;

    public RecordatorioCitaController(OftalvistaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedItemsResponse<RecordatorioCitaItemsDto>>> Listar(
        [FromQuery] RecordatorioCitaListQuery query
    )
    {
        var pageSize = ApiResponseFactory.NormalizePageSize(query.PageSize);
        var skip = ApiResponseFactory.NormalizeSkip(query.Skip);
        var from = Formatters.ParseDateRangeStart(query.FechaRegistroDesde);
        var to = Formatters.ParseDateRangeEnd(query.FechaRegistroHasta);
        var baseQuery = _dbContext.RecordatoriosCita.AsNoTracking().AsQueryable();

        if (int.TryParse(query.IdCita, out var idCita))
        {
            baseQuery = baseQuery.Where(item => item.IdCita == idCita);
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
            ? baseQuery.OrderByDescending(item => item.IdRecordatorioCita)
            : baseQuery.OrderBy(item => item.IdRecordatorioCita);

        var count = await baseQuery.CountAsync();
        var data = await baseQuery
            .Skip(skip)
            .Take(pageSize)
            .Select(item => new RecordatorioCitaItemsDto
            {
                IdRecordatorioCita = item.IdRecordatorioCita,
                Guid = item.Guid.ToString(),
                IdCita = item.IdCita,
                IdEstadoRecordatorio = item.IdEstadoRecordatorio,
                FechaProgramada = Formatters.ToDateText(item.FechaProgramada),
                FechaEnvio = Formatters.ToDateText(item.FechaEnvio),
                Mensaje = item.Mensaje,
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
    public async Task<ActionResult<RecordatorioCitaItemsDto>> Obtener(string guid)
    {
        var entity = await _dbContext.RecordatoriosCita.AsNoTracking().FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Recordatorio de cita no encontrado." });
        }

        return Ok(Map(entity));
    }

    [HttpPost]
    public async Task<ActionResult<RecordatorioCitaItemsDto>> Crear(
        [FromBody] CreateRecordatorioCitaRequest request
    )
    {
        var citaExists = await _dbContext.Citas.AnyAsync(item => item.IdCita == request.IdCita);
        if (!citaExists)
        {
            return BadRequest(new { message = "La cita asociada no existe." });
        }

        var entity = new RecordatorioCita
        {
            IdCita = request.IdCita,
            IdEstadoRecordatorio = request.IdEstadoRecordatorio,
            FechaProgramada = Formatters.ParseDateTime(request.FechaProgramada),
            FechaEnvio = Formatters.ParseNullableDateTime(request.FechaEnvio),
            Mensaje = request.Mensaje,
            IdTblEstadoVigencia = request.IdTblEstadoVigencia,
            EsEliminado = false,
        };

        _dbContext.RecordatoriosCita.Add(entity);
        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpPut("{guid}")]
    public async Task<ActionResult<RecordatorioCitaItemsDto>> Editar(
        string guid,
        [FromBody] UpdateRecordatorioCitaRequest request
    )
    {
        var entity = await _dbContext.RecordatoriosCita.FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Recordatorio de cita no encontrado." });
        }

        entity.IdCita = request.IdCita;
        entity.IdEstadoRecordatorio = request.IdEstadoRecordatorio;
        entity.FechaProgramada = Formatters.ParseDateTime(request.FechaProgramada);
        entity.FechaEnvio = Formatters.ParseNullableDateTime(request.FechaEnvio);
        entity.Mensaje = request.Mensaje;
        entity.IdTblEstadoVigencia = request.IdTblEstadoVigencia;

        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpDelete("{guid}")]
    public async Task<IActionResult> Eliminar(string guid)
    {
        var entity = await _dbContext.RecordatoriosCita.FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Recordatorio de cita no encontrado." });
        }

        entity.EsEliminado = true;
        entity.IdTblEstadoVigencia = 2;
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static RecordatorioCitaItemsDto Map(RecordatorioCita item) =>
        new()
        {
            IdRecordatorioCita = item.IdRecordatorioCita,
            Guid = item.Guid.ToString(),
            IdCita = item.IdCita,
            IdEstadoRecordatorio = item.IdEstadoRecordatorio,
            FechaProgramada = Formatters.ToDateText(item.FechaProgramada),
            FechaEnvio = Formatters.ToDateText(item.FechaEnvio),
            Mensaje = item.Mensaje,
            IdTblEstadoVigencia = item.IdTblEstadoVigencia,
        };
}
