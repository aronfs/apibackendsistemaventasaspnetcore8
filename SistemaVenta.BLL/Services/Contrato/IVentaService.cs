using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DTO;


namespace SistemaVenta.BLL.Services.Contrato
{
    public interface IVentaService
    {
        Task<VentaDTO> Registrar(VentaDTO venta);
        Task<List<VentaDTO>> Historial(string buscarPro, string numeroVenta, string fechaInicio, string fechaFin);
        Task<List<ReporteDTO>> Reporte(string fechaInicio, string fechaFin);
        
    }
}
