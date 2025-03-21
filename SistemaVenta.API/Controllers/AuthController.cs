using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.BLL.Services.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.API.Utilidad;
using SistemaVenta.BLL.Services;
namespace SistemaVenta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDTO authDTO)
        {
            var usuario = await _authService.ValidarUsuario(authDTO);

            if (usuario == null)
            {
                return Unauthorized(new { mensaje = "Credenciales incorrectas." });
            }

            var token = await _authService.GenerarToken(usuario);

            return Ok(new
            {
                token,
                usuario = new
                {
                    usuario.NombreCompleto,
                    usuario.Correo,
                    usuario.IdRol
                }
            });
        }
    }
}
