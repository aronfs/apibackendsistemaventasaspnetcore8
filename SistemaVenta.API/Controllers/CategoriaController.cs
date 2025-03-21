using Microsoft.AspNetCore.Mvc;
using SistemaVenta.BLL.Services.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.API.Utilidad;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet("Lista")]
        public async Task<IActionResult> Lista()
        {
            var response = new Response<List<CategoriaDTO>>();
            try
            {
                var categorias = await _categoriaService.Lista();

                if (categorias == null || !categorias.Any())
                {
                    response.status = false;
                    response.msg = "No se encontraron categorías.";
                    return NotFound(response);
                }

                response.status = true;
                response.Value = categorias;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.status = false;
                response.msg = $"Error al obtener la lista de categorías: {ex.Message}";
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
