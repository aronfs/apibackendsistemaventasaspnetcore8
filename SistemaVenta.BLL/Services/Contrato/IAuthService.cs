using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.DTO;
using SistemaVenta.Model.Models;

namespace SistemaVenta.BLL.Services.Contrato
{
    public interface IAuthService
    {
        Task<string> GenerarToken(Usuario usuario);
        Task<Usuario> ValidarUsuario(AuthDTO authDTO);
    }
}
