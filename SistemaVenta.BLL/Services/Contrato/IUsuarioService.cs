using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DTO;

namespace SistemaVenta.BLL.Services.Contrato
{
    public interface IUsuarioService
    {
        Task<List<UsuarioDTO>> Lista();
        Task<List<UsuarioDTO>> ListaNoActivos();
        Task<SesionDTO> ValidarCredenciales(string correo, string clave);
        Task<UsuarioDTO> Crear(UsuarioDTO modelo);
        Task<bool> Editar(UsuarioDTO modelo);
        Task<bool> ActivarUsuario(int id);
        Task<bool> Eliminar(int id);
        Task<bool> EliminadoLogico(int id);
        Task<string?> ObtenerFotoPorCorreo(string correo); // Nuevo método añadido
    }
}
