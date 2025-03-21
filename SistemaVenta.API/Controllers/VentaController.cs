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
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentaController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        [HttpPost]
        [Route("Registrar")]
        public async Task<IActionResult> Registrar([FromBody] VentaDTO modelo)
        {
            var response = new Response<VentaDTO>();
            try
            {
                response.status = true;
                response.Value = await _ventaService.Registrar(modelo);
            }
            catch (Exception ex)
            {
                response.status = false;
                response.msg = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("Historial")]
        public async Task<IActionResult> Historial(string buscarPro, string? numeroVenta, string? FechaInicio, string? FechaFin)
        {
            var response = new Response<List<VentaDTO>>();
            numeroVenta = numeroVenta is null ? "": numeroVenta;
            FechaInicio = FechaInicio is null ? "" : FechaInicio;
            FechaFin = FechaFin is null ? "" : FechaFin;
            try
            {
                response.status = true;
                response.Value = await _ventaService.Historial(buscarPro, numeroVenta, FechaInicio,FechaFin);
            }
            catch (Exception ex)
            {
                response.status = false;
                response.msg = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("Reporte")]
        public async Task<IActionResult> Reporte( string? FechaInicio, string? FechaFin)
        {
            var response = new Response<List<ReporteDTO>>();
           
            
            try
            {
                response.status = true;
                response.Value = await _ventaService.Reporte( FechaInicio, FechaFin);
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
