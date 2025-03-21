using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.BLL.Services.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.API.Utilidad;
using SistemaVenta.BLL.Services;
using Microsoft.AspNetCore.Authorization;
namespace SistemaVenta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashBoardController : ControllerBase
    {
        private readonly IDashBoardService _dashBoardService;
        public DashBoardController(IDashBoardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }

        [HttpGet]
        [Route("Resumen")]
        public async Task<IActionResult> Resumen()
        {
            var response = new Response<DashBoardDTO>();

            try
            {
                response.status = true;
                response.Value = await _dashBoardService.Resumen();
            }
            catch (Exception ex)
            {
                response.status = false;
                response.msg = ex.Message;
            }
            return Ok(response);
        }
    }
}
