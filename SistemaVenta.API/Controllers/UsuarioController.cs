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
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var response = new Response<List<UsuarioDTO>>();

            try
            {
                response.status = true;
                response.Value = await _usuarioService.Lista();
            }
            catch (Exception ex)
            {
                response.status = false;
                response.msg = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("ObtenerFotoPorCorreo/{correo}")]
        public async Task<IActionResult> ObtenerFotoPorCorreo(string correo)
        {
            var response = new Response<string>();
            try
            {
                var fotoBase64 = await _usuarioService.ObtenerFotoPorCorreo(correo);
                if (fotoBase64 != null)
                {
                    response.status = true;
                    response.Value = fotoBase64;
                }
                else
                {
                    response.status = false;
                    response.msg = "No se encontró la foto del usuario.";
                }
            }
            catch (Exception ex)
            {
                response.status = false;
                response.msg = ex.Message;
            }
            return Ok(response);
        }


        [HttpPost]
        [Route("IniciarSesion")]
        public async Task<IActionResult> InciarSesion([FromBody] LoginDTO modelo)
        {
            var response = new Response<SesionDTO>();
            try
            {
                response.status = true;
                response.Value = await _usuarioService.ValidarCredenciales(modelo.Correo, modelo.Clave);
            }
            catch (Exception ex)
            {
                response.status = false;
                response.msg = ex.Message;
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar([FromBody] UsuarioDTO modelo)
        {
            var response = new Response<UsuarioDTO>();
            try
            {
                response.status = true;
                response.Value = await _usuarioService.Crear(modelo);
            }
            catch (Exception ex)
            {
                response.status = false;
                response.msg = ex.Message;
            }
            return Ok(response);
        }

        [HttpPut]
        [Route("Editar")]
        public async Task<IActionResult> Editar([FromBody] UsuarioDTO modelo)
        {
            var response = new Response<bool>();
            try
            {
                response.status = true;
                response.Value = await _usuarioService.Editar(modelo);
            }
            catch (Exception ex)
            {
                response.status = false;
                response.msg = ex.Message;
            }
            return Ok(response);
        }


        [HttpDelete]
        [Route("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var response = new Response<bool>();
            try
            {
                response.status = true;
                response.Value = await _usuarioService.Eliminar(id);
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