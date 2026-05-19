using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oftalvista.Api.Contracts;
using Oftalvista.Api.Data;
using Oftalvista.Api.Domain;
using Oftalvista.Api.Helpers;

namespace Oftalvista.Api.Controllers;

[ApiController]
[Route("api/v1/especialidad-medica")]
public class EspecialidadMedicaController : ControllerBase
{
    private readonly OftalvistaDbContext _dbContext;

    public EspecialidadMedicaController(OftalvistaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedItemsResponse<EspecialidadMedicaItemsDto>>> Listar(
        [FromQuery] EspecialidadMedicaListQuery query
    )
    {
        var pageSize = ApiResponseFactory.NormalizePageSize(query.PageSize);
        var skip = ApiResponseFactory.NormalizeSkip(query.Skip);
        var from = Formatters.ParseDateRangeStart(query.FechaRegistroDesde);
        var to = Formatters.ParseDateRangeEnd(query.FechaRegistroHasta);
        var baseQuery = _dbContext.EspecialidadesMedicas.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Codigo))
        {
            baseQuery = baseQuery.Where(item => item.Codigo.Contains(query.Codigo));
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
            ? baseQuery.OrderByDescending(item => item.IdEspecialidadMedica)
            : baseQuery.OrderBy(item => item.IdEspecialidadMedica);

        var count = await baseQuery.CountAsync();
        var data = await baseQuery
            .Skip(skip)
            .Take(pageSize)
            .Select(item => new EspecialidadMedicaItemsDto
            {
                IdEspecialidadMedica = item.IdEspecialidadMedica,
                Guid = item.Guid.ToString(),
                Codigo = item.Codigo,
                Nombre = item.Nombre,
                Descripcion = item.Descripcion,
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
    public async Task<ActionResult<EspecialidadMedicaItemsDto>> Obtener(string guid)
    {
        var entity = await _dbContext.EspecialidadesMedicas.AsNoTracking().FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Especialidad no encontrada." });
        }

        return Ok(Map(entity));
    }

    [HttpPost]
    public async Task<ActionResult<EspecialidadMedicaItemsDto>> Crear(
        [FromBody] CreateEspecialidadMedicaRequest request
    )
    {
        var exists = await _dbContext.EspecialidadesMedicas.AnyAsync(item => item.Codigo == request.Codigo);
        if (exists)
        {
            return BadRequest(new { message = "Ya existe una especialidad con el mismo codigo." });
        }

        var entity = new EspecialidadMedica
        {
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            IdTblEstadoVigencia = request.IdTblEstadoVigencia,
            EsEliminado = false,
        };

        _dbContext.EspecialidadesMedicas.Add(entity);
        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpPut("{guid}")]
    public async Task<ActionResult<EspecialidadMedicaItemsDto>> Editar(
        string guid,
        [FromBody] UpdateEspecialidadMedicaRequest request
    )
    {
        var entity = await _dbContext.EspecialidadesMedicas.FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Especialidad no encontrada." });
        }

        var duplicate = await _dbContext.EspecialidadesMedicas.AnyAsync(item =>
            item.IdEspecialidadMedica != entity.IdEspecialidadMedica && item.Codigo == request.Codigo
        );
        if (duplicate)
        {
            return BadRequest(new { message = "Ya existe una especialidad con el mismo codigo." });
        }

        entity.Codigo = request.Codigo;
        entity.Nombre = request.Nombre;
        entity.Descripcion = request.Descripcion;
        entity.IdTblEstadoVigencia = request.IdTblEstadoVigencia;

        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpDelete("{guid}")]
    public async Task<IActionResult> Eliminar(string guid)
    {
        var entity = await _dbContext.EspecialidadesMedicas.FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Especialidad no encontrada." });
        }

        entity.EsEliminado = true;
        entity.IdTblEstadoVigencia = 2;
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static EspecialidadMedicaItemsDto Map(EspecialidadMedica item) =>
        new()
        {
            IdEspecialidadMedica = item.IdEspecialidadMedica,
            Guid = item.Guid.ToString(),
            Codigo = item.Codigo,
            Nombre = item.Nombre,
            Descripcion = item.Descripcion,
            IdTblEstadoVigencia = item.IdTblEstadoVigencia,
        };
}
