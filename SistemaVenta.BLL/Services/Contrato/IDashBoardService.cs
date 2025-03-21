using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DTO;
namespace SistemaVenta.BLL.Services.Contrato
{
    public interface IDashBoardService
    {
        Task<DashBoardDTO> Resumen();
    }
}
