using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.DTO;


namespace SistemaVenta.BLL.Services.Contrato
{
    public interface IRolService
    {
        Task<List<RolDTO>> Lista();

        Task<string?> ObtenerRolNombre(string idRol); // Nuevo método añadido

    }
}
