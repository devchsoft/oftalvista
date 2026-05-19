using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oftalvista.Api.Contracts;
using Oftalvista.Api.Data;
using Oftalvista.Api.Domain;
using Oftalvista.Api.Helpers;

namespace Oftalvista.Api.Controllers;

[ApiController]
[Route("api/v1/paciente")]
public class PacienteController : ControllerBase
{
    private readonly OftalvistaDbContext _dbContext;

    public PacienteController(OftalvistaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedItemsResponse<PacienteItemsDto>>> Listar(
        [FromQuery] PacienteListQuery query
    )
    {
        var pageSize = ApiResponseFactory.NormalizePageSize(query.PageSize);
        var skip = ApiResponseFactory.NormalizeSkip(query.Skip);
        var from = Formatters.ParseDateRangeStart(query.FechaRegistroDesde);
        var to = Formatters.ParseDateRangeEnd(query.FechaRegistroHasta);
        var baseQuery = _dbContext.Pacientes.AsNoTracking().AsQueryable();

        if (int.TryParse(query.IdUsuario, out var idUsuario))
        {
            baseQuery = baseQuery.Where(item => item.IdUsuario == idUsuario);
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
            ? baseQuery.OrderByDescending(item => item.IdPaciente)
            : baseQuery.OrderBy(item => item.IdPaciente);

        var count = await baseQuery.CountAsync();
        var data = await baseQuery
            .Skip(skip)
            .Take(pageSize)
            .Select(item => new PacienteItemsDto
            {
                IdPaciente = item.IdPaciente,
                Guid = item.Guid.ToString(),
                IdUsuario = item.IdUsuario,
                FechaNacimiento = Formatters.ToDateText(item.FechaNacimiento),
                Sexo = item.Sexo,
                Direccion = item.Direccion,
                ContactoEmergencia = item.ContactoEmergencia,
                TelefonoEmergencia = item.TelefonoEmergencia,
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
    public async Task<ActionResult<PacienteItemsDto>> Obtener(string guid)
    {
        var entity = await _dbContext.Pacientes.AsNoTracking().FirstOrDefaultAsync(item =>
            item.Guid.ToString() == guid
        );
        if (entity is null)
        {
            return NotFound(new { message = "Paciente no encontrado." });
        }

        return Ok(Map(entity));
    }

    [HttpPost]
    public async Task<ActionResult<PacienteItemsDto>> Crear([FromBody] CreatePacienteRequest request)
    {
        var usuarioExists = await _dbContext.Usuarios.AnyAsync(item => item.IdUsuario == request.IdUsuario);
        if (!usuarioExists)
        {
            return BadRequest(new { message = "Usuario no valido para el paciente." });
        }

        var entity = new Paciente
        {
            IdUsuario = request.IdUsuario,
            FechaNacimiento = Formatters.ParseNullableDateOnly(request.FechaNacimiento),
            Sexo = request.Sexo,
            Direccion = request.Direccion,
            ContactoEmergencia = request.ContactoEmergencia,
            TelefonoEmergencia = request.TelefonoEmergencia,
            IdTblEstadoVigencia = request.IdTblEstadoVigencia,
            EsEliminado = false,
        };

        _dbContext.Pacientes.Add(entity);
        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpPut("{guid}")]
    public async Task<ActionResult<PacienteItemsDto>> Editar(
        string guid,
        [FromBody] UpdatePacienteRequest request
    )
    {
        var entity = await _dbContext.Pacientes.FirstOrDefaultAsync(item => item.Guid.ToString() == guid);
        if (entity is null)
        {
            return NotFound(new { message = "Paciente no encontrado." });
        }

        entity.IdUsuario = request.IdUsuario;
        entity.FechaNacimiento = Formatters.ParseNullableDateOnly(request.FechaNacimiento);
        entity.Sexo = request.Sexo;
        entity.Direccion = request.Direccion;
        entity.ContactoEmergencia = request.ContactoEmergencia;
        entity.TelefonoEmergencia = request.TelefonoEmergencia;
        entity.IdTblEstadoVigencia = request.IdTblEstadoVigencia;

        await _dbContext.SaveChangesAsync();

        return Ok(Map(entity));
    }

    [HttpDelete("{guid}")]
    public async Task<IActionResult> Eliminar(string guid)
    {
        var entity = await _dbContext.Pacientes.FirstOrDefaultAsync(item => item.Guid.ToString() == guid);
        if (entity is null)
        {
            return NotFound(new { message = "Paciente no encontrado." });
        }

        entity.EsEliminado = true;
        entity.IdTblEstadoVigencia = 2;
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static PacienteItemsDto Map(Paciente item) =>
        new()
        {
            IdPaciente = item.IdPaciente,
            Guid = item.Guid.ToString(),
            IdUsuario = item.IdUsuario,
            FechaNacimiento = Formatters.ToDateText(item.FechaNacimiento),
            Sexo = item.Sexo,
            Direccion = item.Direccion,
            ContactoEmergencia = item.ContactoEmergencia,
            TelefonoEmergencia = item.TelefonoEmergencia,
            IdTblEstadoVigencia = item.IdTblEstadoVigencia,
        };
}
