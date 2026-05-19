using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oftalvista.Api.Contracts;
using Oftalvista.Api.Data;
using Oftalvista.Api.Domain;
using Oftalvista.Api.Helpers;

namespace Oftalvista.Api.Controllers;

[ApiController]
[Route("api/v1/medico")]
public class MedicoController : ControllerBase
{
    private readonly OftalvistaDbContext _dbContext;

    public MedicoController(OftalvistaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedItemsResponse<MedicoItemsDto>>> Listar(
        [FromQuery] MedicoListQuery query
    )
    {
        var pageSize = ApiResponseFactory.NormalizePageSize(query.PageSize);
        var skip = ApiResponseFactory.NormalizeSkip(query.Skip);
        var from = Formatters.ParseDateRangeStart(query.FechaRegistroDesde);
        var to = Formatters.ParseDateRangeEnd(query.FechaRegistroHasta);
        var baseQuery = _dbContext.Medicos.AsNoTracking().AsQueryable();

        if (int.TryParse(query.IdUsuario, out var idUsuario))
        {
            baseQuery = baseQuery.Where(item => item.IdUsuario == idUsuario);
        }

        if (int.TryParse(query.IdEspecialidadMedica, out var idEspecialidad))
        {
            baseQuery = baseQuery.Where(item => item.IdEspecialidadMedica == idEspecialidad);
        }

        if (!string.IsNullOrWhiteSpace(query.Cmp))
        {
            baseQuery = baseQuery.Where(item => (item.Cmp ?? string.Empty).Contains(query.Cmp));
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
            ? baseQuery.OrderByDescending(item => item.IdMedico)
            : baseQuery.OrderBy(item => item.IdMedico);

        var count = await baseQuery.CountAsync();
        var data = await baseQuery
            .Skip(skip)
            .Take(pageSize)
            .Select(item => new MedicoItemsDto
            {
                IdMedico = item.IdMedico,
                Guid = item.Guid.ToString(),
                IdUsuario = item.IdUsuario,
                IdEspecialidadMedica = item.IdEspecialidadMedica,
                Cmp = item.Cmp,
                PerfilProfesional = item.PerfilProfesional,
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
    public async Task<ActionResult<MedicoItemsDto>> Obtener(string guid)
    {
        var entity = await _dbContext.Medicos.AsNoTracking().FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Medico no encontrado." });
        }

        return Ok(Map(entity));
    }

    [HttpPost]
    public async Task<ActionResult<MedicoItemsDto>> Crear([FromBody] CreateMedicoRequest request)
    {
        var usuarioExists = await _dbContext.Usuarios.AnyAsync(item => item.IdUsuario == request.IdUsuario);
        var especialidadExists = await _dbContext.EspecialidadesMedicas.AnyAsync(item =>
            item.IdEspecialidadMedica == request.IdEspecialidadMedica
        );
        if (!usuarioExists || !especialidadExists)
        {
            return BadRequest(new { message = "Usuario o especialidad medica no valida." });
        }

        var entity = new Medico
        {
            IdUsuario = request.IdUsuario,
            IdEspecialidadMedica = request.IdEspecialidadMedica,
            Cmp = request.Cmp,
            PerfilProfesional = request.PerfilProfesional,
            IdTblEstadoVigencia = request.IdTblEstadoVigencia,
            EsEliminado = false,
        };

        _dbContext.Medicos.Add(entity);
        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpPut("{guid}")]
    public async Task<ActionResult<MedicoItemsDto>> Editar(string guid, [FromBody] UpdateMedicoRequest request)
    {
        var entity = await _dbContext.Medicos.FirstOrDefaultAsync(item => item.Guid.ToString() == guid);
        if (entity is null)
        {
            return NotFound(new { message = "Medico no encontrado." });
        }

        entity.IdUsuario = request.IdUsuario;
        entity.IdEspecialidadMedica = request.IdEspecialidadMedica;
        entity.Cmp = request.Cmp;
        entity.PerfilProfesional = request.PerfilProfesional;
        entity.IdTblEstadoVigencia = request.IdTblEstadoVigencia;

        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpDelete("{guid}")]
    public async Task<IActionResult> Eliminar(string guid)
    {
        var entity = await _dbContext.Medicos.FirstOrDefaultAsync(item => item.Guid.ToString() == guid);
        if (entity is null)
        {
            return NotFound(new { message = "Medico no encontrado." });
        }

        entity.EsEliminado = true;
        entity.IdTblEstadoVigencia = 2;
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static MedicoItemsDto Map(Medico item) =>
        new()
        {
            IdMedico = item.IdMedico,
            Guid = item.Guid.ToString(),
            IdUsuario = item.IdUsuario,
            IdEspecialidadMedica = item.IdEspecialidadMedica,
            Cmp = item.Cmp,
            PerfilProfesional = item.PerfilProfesional,
            IdTblEstadoVigencia = item.IdTblEstadoVigencia,
        };
}
