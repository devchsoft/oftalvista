using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oftalvista.Api.Contracts;
using Oftalvista.Api.Data;
using Oftalvista.Api.Helpers;

namespace Oftalvista.Api.Controllers;

[ApiController]
[Route("api/v1/catalogo")]
public class CatalogoController : ControllerBase
{
    private readonly OftalvistaDbContext _dbContext;

    public CatalogoController(OftalvistaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("tipos-usuario")]
    public ActionResult<IReadOnlyCollection<CatalogoItem>> GetTiposUsuario() => Ok(Catalogos.TiposUsuario());

    [HttpGet("tipos-documento")]
    public ActionResult<IReadOnlyCollection<CatalogoItem>> GetTiposDocumento() =>
        Ok(Catalogos.TiposDocumento());

    [HttpGet("estados-vigencia")]
    public ActionResult<IReadOnlyCollection<CatalogoItem>> GetEstadosVigencia() =>
        Ok(Catalogos.EstadosVigencia());

    [HttpGet("estados-cita")]
    public ActionResult<IReadOnlyCollection<CatalogoItem>> GetEstadosCita() => Ok(Catalogos.EstadosCita());

    [HttpGet("modalidades-cita")]
    public ActionResult<IReadOnlyCollection<CatalogoItem>> GetModalidadesCita() =>
        Ok(Catalogos.ModalidadesCita());

    [HttpGet("metodos-pago")]
    public ActionResult<IReadOnlyCollection<CatalogoItem>> GetMetodosPago() => Ok(Catalogos.MetodosPago());

    [HttpGet("estados-pago")]
    public ActionResult<IReadOnlyCollection<CatalogoItem>> GetEstadosPago() => Ok(Catalogos.EstadosPago());

    [HttpGet("estados-recordatorio")]
    public ActionResult<IReadOnlyCollection<CatalogoItem>> GetEstadosRecordatorio() =>
        Ok(Catalogos.EstadosRecordatorio());

    [HttpGet("especialidades")]
    public async Task<ActionResult<IReadOnlyCollection<CatalogoItem>>> GetEspecialidades()
    {
        var data = await _dbContext
            .EspecialidadesMedicas.AsNoTracking()
            .Where(item => item.IdTblEstadoVigencia == 1)
            .OrderBy(item => item.Nombre)
            .Select(item => new CatalogoItem { Value = item.IdEspecialidadMedica, Text = item.Nombre })
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("medicos")]
    public async Task<ActionResult<IReadOnlyCollection<CatalogoItem>>> GetMedicos()
    {
        var data = await (
            from medico in _dbContext.Medicos.AsNoTracking()
            join usuario in _dbContext.Usuarios.AsNoTracking() on medico.IdUsuario equals usuario.IdUsuario
            where medico.IdTblEstadoVigencia == 1 && usuario.IdTblEstadoVigencia == 1
            orderby usuario.Nombres, usuario.Apellidos
            select new CatalogoItem
            {
                Value = medico.IdMedico,
                Text = (usuario.Nombres + " " + usuario.Apellidos).Trim(),
            }
        ).ToListAsync();

        return Ok(data);
    }

    [HttpGet("pacientes")]
    public async Task<ActionResult<IReadOnlyCollection<CatalogoItem>>> GetPacientes()
    {
        var data = await (
            from paciente in _dbContext.Pacientes.AsNoTracking()
            join usuario in _dbContext.Usuarios.AsNoTracking() on paciente.IdUsuario equals usuario.IdUsuario
            where paciente.IdTblEstadoVigencia == 1 && usuario.IdTblEstadoVigencia == 1
            orderby usuario.Nombres, usuario.Apellidos
            select new CatalogoItem
            {
                Value = paciente.IdPaciente,
                Text = (usuario.Nombres + " " + usuario.Apellidos).Trim(),
            }
        ).ToListAsync();

        return Ok(data);
    }

    [HttpGet("usuarios")]
    public async Task<ActionResult<IReadOnlyCollection<CatalogoItem>>> GetUsuarios()
    {
        var data = await _dbContext
            .Usuarios.AsNoTracking()
            .Where(item => item.IdTblEstadoVigencia == 1)
            .OrderBy(item => item.Nombres)
            .ThenBy(item => item.Apellidos)
            .Select(item => new CatalogoItem
            {
                Value = item.IdUsuario,
                Text = (item.Nombres + " " + item.Apellidos).Trim(),
            })
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("agendas-medico/{id:int}")]
    public async Task<ActionResult<IReadOnlyCollection<CatalogoItem>>> GetAgendasMedico(int id)
    {
        var data = await _dbContext
            .AgendasMedicas.AsNoTracking()
            .Where(item => item.IdMedico == id && item.IdTblEstadoVigencia == 1 && item.EsDisponible)
            .OrderBy(item => item.Fecha)
            .ThenBy(item => item.HoraInicio)
            .Select(item => new CatalogoItem
            {
                Value = item.IdAgendaMedica,
                Text =
                    $"{Formatters.ToDateText(item.Fecha)} {Formatters.ToTimeText(item.HoraInicio)} - {Formatters.ToTimeText(item.HoraFin)}",
            })
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("citas")]
    public async Task<ActionResult<IReadOnlyCollection<CatalogoItem>>> GetCitas()
    {
        var data = await _dbContext
            .Citas.AsNoTracking()
            .Where(item => item.IdTblEstadoVigencia == 1)
            .OrderByDescending(item => item.FechaCita)
            .ThenBy(item => item.HoraCita)
            .Select(item => new CatalogoItem
            {
                Value = item.IdCita,
                Text =
                    $"Cita {item.IdCita} - {Formatters.ToDateText(item.FechaCita)} {Formatters.ToTimeText(item.HoraCita)}",
            })
            .ToListAsync();

        return Ok(data);
    }
}
