using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oftalvista.Api.Contracts;
using Oftalvista.Api.Data;
using Oftalvista.Api.Helpers;

namespace Oftalvista.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly OftalvistaDbContext _dbContext;

    public AuthController(OftalvistaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var correo = request.Correo.Trim().ToLowerInvariant();

        var usuario = await _dbContext.Usuarios.AsNoTracking().FirstOrDefaultAsync(item =>
            item.Correo.ToLower() == correo
            && item.ClaveHash == request.ClaveHash
            && item.IdTblEstadoVigencia == 1
        );

        if (usuario is null)
        {
            return Unauthorized(new { message = "Credenciales invalidas." });
        }

        return Ok(
            new LoginResponse
            {
                Token = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(
                        $"ov:{usuario.IdUsuario}:{DateTime.UtcNow:O}"
                    )
                ),
                RefreshToken = Guid.NewGuid().ToString("N"),
                IdUsuario = usuario.IdUsuario,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                IdTipoUsuario = usuario.IdTipoUsuario,
                TipoUsuario = Catalogos.TipoUsuarioTexto(usuario.IdTipoUsuario),
            }
        );
    }
}
