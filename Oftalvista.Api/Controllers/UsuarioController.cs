using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oftalvista.Api.Contracts;
using Oftalvista.Api.Data;
using Oftalvista.Api.Domain;
using Oftalvista.Api.Helpers;

namespace Oftalvista.Api.Controllers;

[ApiController]
[Route("api/v1/usuario")]
public class UsuarioController : ControllerBase
{
    private readonly OftalvistaDbContext _dbContext;

    public UsuarioController(OftalvistaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedItemsResponse<UsuarioItemsDto>>> Listar(
        [FromQuery] UsuarioListQuery query
    )
    {
        var pageSize = ApiResponseFactory.NormalizePageSize(query.PageSize);
        var skip = ApiResponseFactory.NormalizeSkip(query.Skip);
        var from = Formatters.ParseDateRangeStart(query.FechaRegistroDesde);
        var to = Formatters.ParseDateRangeEnd(query.FechaRegistroHasta);
        var baseQuery = _dbContext.Usuarios.AsNoTracking().AsQueryable();

        if (int.TryParse(query.IdTipoUsuario, out var idTipoUsuario))
        {
            baseQuery = baseQuery.Where(item => item.IdTipoUsuario == idTipoUsuario);
        }

        if (int.TryParse(query.IdTipoDocumento, out var idTipoDocumento))
        {
            baseQuery = baseQuery.Where(item => item.IdTipoDocumento == idTipoDocumento);
        }

        if (!string.IsNullOrWhiteSpace(query.NumeroDocumento))
        {
            baseQuery = baseQuery.Where(item =>
                (item.NumeroDocumento ?? string.Empty).Contains(query.NumeroDocumento)
            );
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
            ? baseQuery.OrderByDescending(item => item.IdUsuario)
            : baseQuery.OrderBy(item => item.IdUsuario);

        var count = await baseQuery.CountAsync();
        var data = await baseQuery
            .Skip(skip)
            .Take(pageSize)
            .Select(item => new UsuarioItemsDto
            {
                IdUsuario = item.IdUsuario,
                Guid = item.Guid.ToString(),
                IdTipoUsuario = item.IdTipoUsuario,
                IdTipoDocumento = item.IdTipoDocumento,
                NumeroDocumento = item.NumeroDocumento,
                Nombres = item.Nombres,
                Apellidos = item.Apellidos,
                Correo = item.Correo,
                Telefono = item.Telefono,
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
    public async Task<ActionResult<UsuarioItemsDto>> Obtener(string guid)
    {
        var entity = await _dbContext.Usuarios.AsNoTracking().FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );

        if (entity is null)
        {
            return NotFound(new { message = "Usuario no encontrado." });
        }

        return Ok(Map(entity));
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioItemsDto>> Crear([FromBody] CreateUsuarioRequest request)
    {
        var correo = request.Correo.Trim().ToLowerInvariant();
        var exists = await _dbContext.Usuarios.AnyAsync(item => item.Correo.ToLower() == correo);
        if (exists)
        {
            return BadRequest(new { message = "Ya existe un usuario con el mismo correo." });
        }

        var entity = new Usuario
        {
            IdTipoUsuario = request.IdTipoUsuario,
            IdTipoDocumento = request.IdTipoDocumento,
            NumeroDocumento = request.NumeroDocumento,
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            Correo = correo,
            ClaveHash = request.ClaveHash,
            Telefono = request.Telefono,
            IdTblEstadoVigencia = request.IdTblEstadoVigencia,
            EsEliminado = false,
        };

        _dbContext.Usuarios.Add(entity);
        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpPut("{guid}")]
    public async Task<ActionResult<UsuarioItemsDto>> Editar(
        string guid,
        [FromBody] UpdateUsuarioRequest request
    )
    {
        var entity = await _dbContext.Usuarios.FirstOrDefaultAsync(item => item.Guid.ToString() == guid);
        if (entity is null)
        {
            return NotFound(new { message = "Usuario no encontrado." });
        }

        var correo = request.Correo.Trim().ToLowerInvariant();
        var correoDuplicado = await _dbContext.Usuarios.AnyAsync(item =>
            item.IdUsuario != entity.IdUsuario && item.Correo.ToLower() == correo
        );
        if (correoDuplicado)
        {
            return BadRequest(new { message = "Ya existe un usuario con el mismo correo." });
        }

        entity.IdTipoUsuario = request.IdTipoUsuario;
        entity.IdTipoDocumento = request.IdTipoDocumento;
        entity.NumeroDocumento = request.NumeroDocumento;
        entity.Nombres = request.Nombres;
        entity.Apellidos = request.Apellidos;
        entity.Correo = correo;
        entity.Telefono = request.Telefono;
        entity.IdTblEstadoVigencia = request.IdTblEstadoVigencia;

        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpDelete("{guid}")]
    public async Task<IActionResult> Eliminar(string guid)
    {
        var entity = await _dbContext.Usuarios.FirstOrDefaultAsync(item => item.Guid.ToString() == guid);
        if (entity is null)
        {
            return NotFound(new { message = "Usuario no encontrado." });
        }

        entity.EsEliminado = true;
        entity.IdTblEstadoVigencia = 2;
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static UsuarioItemsDto Map(Usuario item) =>
        new()
        {
            IdUsuario = item.IdUsuario,
            Guid = item.Guid.ToString(),
            IdTipoUsuario = item.IdTipoUsuario,
            IdTipoDocumento = item.IdTipoDocumento,
            NumeroDocumento = item.NumeroDocumento,
            Nombres = item.Nombres,
            Apellidos = item.Apellidos,
            Correo = item.Correo,
            Telefono = item.Telefono,
            IdTblEstadoVigencia = item.IdTblEstadoVigencia,
        };
}
