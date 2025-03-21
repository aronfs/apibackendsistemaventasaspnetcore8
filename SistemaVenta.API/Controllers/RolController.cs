using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SistemaVenta.BLL.Services.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.API.Utilidad;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolController : ControllerBase
    {
        private readonly IRolService _rolService;

        public RolController(IRolService rolService)
        {
            _rolService = rolService;
        }


        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista() { 
            var response = new Response<List<RolDTO>>();

            try 
            { 
                response.status = true;
                response.Value = await _rolService.Lista();
            }
            catch (Exception ex)
            {
                response.status = false;
                response.msg = ex.Message;
            }
            return Ok(response);
        }


        [HttpGet("ObtenerRolNombre/{idRol}")]
        public async Task<IActionResult> ObtenerRolNombre(string idRol)
        {
            var rolNombre = await _rolService.ObtenerRolNombre(idRol);

            if (rolNombre != null)
            {
                return Ok(new { status = true, rolNombre });
            }

            return NotFound(new { status = false, message = "Rol no encontrado" });
        }


    }
}
