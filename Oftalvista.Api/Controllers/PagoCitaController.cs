using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oftalvista.Api.Contracts;
using Oftalvista.Api.Data;
using Oftalvista.Api.Domain;
using Oftalvista.Api.Helpers;

namespace Oftalvista.Api.Controllers;

[ApiController]
[Route("api/v1/pago-cita")]
public class PagoCitaController : ControllerBase
{
    private readonly OftalvistaDbContext _dbContext;

    public PagoCitaController(OftalvistaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedItemsResponse<PagoCitaItemsDto>>> Listar(
        [FromQuery] PagoCitaListQuery query
    )
    {
        var pageSize = ApiResponseFactory.NormalizePageSize(query.PageSize);
        var skip = ApiResponseFactory.NormalizeSkip(query.Skip);
        var from = Formatters.ParseDateRangeStart(query.FechaRegistroDesde);
        var to = Formatters.ParseDateRangeEnd(query.FechaRegistroHasta);
        var baseQuery = _dbContext.PagosCita.AsNoTracking().AsQueryable();

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
            ? baseQuery.OrderByDescending(item => item.IdPagoCita)
            : baseQuery.OrderBy(item => item.IdPagoCita);

        var count = await baseQuery.CountAsync();
        var data = await baseQuery
            .Skip(skip)
            .Take(pageSize)
            .Select(item => new PagoCitaItemsDto
            {
                IdPagoCita = item.IdPagoCita,
                Guid = item.Guid.ToString(),
                IdCita = item.IdCita,
                IdMetodoPago = item.IdMetodoPago,
                IdEstadoPago = item.IdEstadoPago,
                Monto = item.Monto,
                FechaPago = Formatters.ToDateText(item.FechaPago),
                NumeroOperacion = item.NumeroOperacion,
                Comprobante = item.Comprobante,
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
    public async Task<ActionResult<PagoCitaItemsDto>> Obtener(string guid)
    {
        var entity = await _dbContext.PagosCita.AsNoTracking().FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Pago de cita no encontrado." });
        }

        return Ok(Map(entity));
    }

    [HttpPost]
    public async Task<ActionResult<PagoCitaItemsDto>> Crear([FromBody] CreatePagoCitaRequest request)
    {
        var citaExists = await _dbContext.Citas.AnyAsync(item => item.IdCita == request.IdCita);
        if (!citaExists)
        {
            return BadRequest(new { message = "La cita asociada no existe." });
        }

        var entity = new PagoCita
        {
            IdCita = request.IdCita,
            IdMetodoPago = request.IdMetodoPago,
            IdEstadoPago = request.IdEstadoPago,
            Monto = request.Monto,
            FechaPago = Formatters.ParseNullableDateTime(request.FechaPago),
            NumeroOperacion = request.NumeroOperacion,
            Comprobante = request.Comprobante,
            IdTblEstadoVigencia = request.IdTblEstadoVigencia,
            EsEliminado = false,
        };

        _dbContext.PagosCita.Add(entity);
        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpPut("{guid}")]
    public async Task<ActionResult<PagoCitaItemsDto>> Editar(
        string guid,
        [FromBody] UpdatePagoCitaRequest request
    )
    {
        var entity = await _dbContext.PagosCita.FirstOrDefaultAsync(item => item.Guid.ToString() == guid);
        if (entity is null)
        {
            return NotFound(new { message = "Pago de cita no encontrado." });
        }

        entity.IdCita = request.IdCita;
        entity.IdMetodoPago = request.IdMetodoPago;
        entity.IdEstadoPago = request.IdEstadoPago;
        entity.Monto = request.Monto;
        entity.FechaPago = Formatters.ParseNullableDateTime(request.FechaPago);
        entity.NumeroOperacion = request.NumeroOperacion;
        entity.Comprobante = request.Comprobante;
        entity.IdTblEstadoVigencia = request.IdTblEstadoVigencia;

        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpDelete("{guid}")]
    public async Task<IActionResult> Eliminar(string guid)
    {
        var entity = await _dbContext.PagosCita.FirstOrDefaultAsync(item => item.Guid.ToString() == guid);
        if (entity is null)
        {
            return NotFound(new { message = "Pago de cita no encontrado." });
        }

        entity.EsEliminado = true;
        entity.IdTblEstadoVigencia = 2;
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static PagoCitaItemsDto Map(PagoCita item) =>
        new()
        {
            IdPagoCita = item.IdPagoCita,
            Guid = item.Guid.ToString(),
            IdCita = item.IdCita,
            IdMetodoPago = item.IdMetodoPago,
            IdEstadoPago = item.IdEstadoPago,
            Monto = item.Monto,
            FechaPago = Formatters.ToDateText(item.FechaPago),
            NumeroOperacion = item.NumeroOperacion,
            Comprobante = item.Comprobante,
            IdTblEstadoVigencia = item.IdTblEstadoVigencia,
        };
}
