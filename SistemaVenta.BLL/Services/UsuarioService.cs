using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Services.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model.Models;

namespace SistemaVenta.BLL.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly IMapper _mapper;

        public UsuarioService(IGenericRepository<Usuario> usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        public async Task<List<UsuarioDTO>> Lista()
        {
            try
            {
                var queryUsuario = await _usuarioRepository.Consultar();
                var listaUsuario = queryUsuario
                    .Include(rol => rol.IdRolNavigation)
                    .Where(u => u.EsActivo == true) // Filtrar solo los usuarios activos
                    .Select(u => new Usuario
                    {
                        IdUsuario = u.IdUsuario,
                        NombreCompleto = u.NombreCompleto,
                        Correo = u.Correo,
                        IdRol = u.IdRol,
                        Clave = u.Clave,
                        EsActivo = u.EsActivo,
                        FechaRegistro = u.FechaRegistro,
                        foto = u.foto
                    })
                    .ToList();

                return _mapper.Map<List<UsuarioDTO>>(listaUsuario);
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<UsuarioDTO>> ListaNoActivos()
        {
            try
            {
                var queryUsuario = await _usuarioRepository.Consultar();
                var listaUsuario = queryUsuario
                    .Include(rol => rol.IdRolNavigation)
                    .Where(u => u.EsActivo == false) // Filtrar solo los usuarios inactivos
                    .Select(u => new Usuario
                    {
                        IdUsuario = u.IdUsuario,
                        NombreCompleto = u.NombreCompleto,
                        Correo = u.Correo,
                        IdRol = u.IdRol,
                        Clave = u.Clave,
                        EsActivo = u.EsActivo,
                        FechaRegistro = u.FechaRegistro,
                        foto = u.foto
                    })
                    .ToList();

                return _mapper.Map<List<UsuarioDTO>>(listaUsuario);
            }
            catch
            {
                throw;
            }
        }

        public async Task<UsuarioDTO> Crear(UsuarioDTO modelo)  
        {
            try        
            {
                // Normalizar el correo
                var correoNormalizado = modelo.Correo?.Trim().ToLower();

                // Validaciones generales
                if (string.IsNullOrWhiteSpace(correoNormalizado))
                    throw new ArgumentException("El correo es obligatorio.");

                if (!IsValidEmail(correoNormalizado))
                    throw new ArgumentException("El formato del correo no es válido.");

                if (string.IsNullOrWhiteSpace(modelo.Clave))
                    throw new ArgumentException("La contraseña es obligatoria.");

                if (!IsPasswordStrong(modelo.Clave))
                    throw new ArgumentException("La contraseña no cumple con los requisitos de seguridad.");

                if (string.IsNullOrWhiteSpace(modelo.NombreCompleto))
                    throw new ArgumentException("El nombre es obligatorio.");

                if (!EsNombreValido(modelo.NombreCompleto))
                    throw new ArgumentException("El nombre no parece válido. Usa un nombre real y bien escrito.");

                if (string.IsNullOrWhiteSpace(modelo.foto))
                    throw new ArgumentException("La foto es obligatoria.");

                // Validar si el correo ya existe en la base de datos (normalizado)
                var existeUsuario = await _usuarioRepository.Consultar(
                    u => u.Correo.Trim().ToLower() == correoNormalizado
                );

                if (existeUsuario.Any())
                    throw new TaskCanceledException("El correo ya está en uso. Por favor, elige otro.");

                var usuarioCrear = _mapper.Map<Usuario>(modelo);

                // Guardar directamente la URL de la imagen y el correo normalizado
                usuarioCrear.foto = modelo.foto;
                usuarioCrear.Correo = correoNormalizado;

                var usuarioCreado = await _usuarioRepository.Crear(usuarioCrear);

                if (usuarioCreado.IdUsuario == 0)
                    throw new TaskCanceledException("El usuario no se pudo crear.");

                var query = await _usuarioRepository.Consultar(u => u.IdUsuario == usuarioCreado.IdUsuario);
                usuarioCreado = query.Include(rol => rol.IdRolNavigation).First();

                return _mapper.Map<UsuarioDTO>(usuarioCreado);
            }
            catch
            {
                throw;
            }
        }






        public async Task<bool> Editar(UsuarioDTO modelo)
        {
            try
            {
                if (modelo == null)
                    throw new ArgumentException("El modelo no puede ser nulo.");

                if (string.IsNullOrWhiteSpace(modelo.NombreCompleto))
                    throw new ArgumentException("El nombre no puede estar vacío.");
                if (!EsNombreValido(modelo.NombreCompleto))
                    throw new ArgumentException("El nombre no parece válido. Usa un nombre real y bien escrito.");
                if (string.IsNullOrWhiteSpace(modelo.Correo) || !EsCorreoValido(modelo.Correo))
                    throw new ArgumentException("El correo no es válido.");

                if (!IsPasswordStrong(modelo.Clave))
                    throw new ArgumentException("La contraseña no cumple con los requisitos de seguridad.");

                var usuarioModelo = _mapper.Map<Usuario>(modelo);
                var usuarioEncontrado = await _usuarioRepository.Obtener(u => u.IdUsuario == usuarioModelo.IdUsuario);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe.");
                var existeUsuario = await _usuarioRepository.Consultar(u => u.Correo == modelo.Correo);
                if (existeUsuario.Any())
                    throw new TaskCanceledException("El correo ya está registrado por otro usuario existente. Por favor, elige otro.");
                usuarioEncontrado.NombreCompleto = usuarioModelo.NombreCompleto;
                usuarioEncontrado.Correo = usuarioModelo.Correo;
                usuarioEncontrado.IdRol = usuarioModelo.IdRol;
                usuarioEncontrado.Clave = usuarioModelo.Clave;
                usuarioEncontrado.EsActivo = usuarioModelo.EsActivo;

                // Validar la foto antes de asignarla
                usuarioEncontrado.foto = !string.IsNullOrWhiteSpace(usuarioModelo.foto) ? usuarioModelo.foto : usuarioEncontrado.foto;

                bool respuesta = await _usuarioRepository.Editar(usuarioEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("El usuario no se pudo editar.");

                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        // Método auxiliar para validar correos electrónicos
        private bool EsCorreoValido(string correo)
        {
            return Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }


        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var usuarioEncontrado = await _usuarioRepository.Obtener(u => u.IdUsuario == id);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no Existe!!!!");

                bool respuesta = await _usuarioRepository.Eliminar(usuarioEncontrado);
                
                if (!respuesta)
                {
                    throw new TaskCanceledException("El usuario no se pudo eliminar!!!!");
                }
                else {
                    throw new ArgumentException("Se elimino permanentemente el usuario");
                }


                    return respuesta;
            }
            catch
            {
                throw;
            }
        }

        public async Task<SesionDTO> ValidarCredenciales(string correo, string clave)
        {
            try
            {
                var usuario = await _usuarioRepository.Obtener(u => u.Correo == correo && u.Clave == clave);

                if (usuario == null)
                    throw new TaskCanceledException("Credenciales inválidas!!!!");

                return _mapper.Map<SesionDTO>(usuario);
            }
            catch
            {
                throw;
            }
        }

     
      
        public async Task<string?> ObtenerFotoPorCorreo(string correo)
        {
            try
            {
                var usuario = await _usuarioRepository.Obtener(u => u.Correo == correo);
                return usuario?.foto;  // Retorna la URL directamente
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la foto del usuario", ex);
            }
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool IsPasswordStrong(string password)
        {
            // Mínimo 8 caracteres, al menos una mayúscula, una minúscula, un número y un carácter especial
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");
        }

        private bool EsNombreValido(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return false;

            // Solo letras, espacios, tildes, y ñ
            var regex = new Regex(@"^[A-ZÁÉÍÓÚÑa-záéíóúñ ]+$");
            if (!regex.IsMatch(nombre))
                return false;

            // No todo mayúsculas ni todo minúsculas
            if (nombre == nombre.ToUpper() || nombre == nombre.ToLower())
                return false;

            // Palabras separadas razonables
            var palabras = nombre.Trim().Split(' ');
            if (palabras.Length == 0 || palabras.Length > 3)
                return false;

            if (palabras.Any(p => p.Length < 2))
                return false;

            // Longitud general aceptable
            if (nombre.Length < 3 || nombre.Length > 50)
                return false;

            return true;
        }

        public async Task<bool> EliminadoLogico(int id)
        {
            try
            {
                var usuarioEncontrado = await _usuarioRepository.Obtener(u => u.IdUsuario == id);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe!!!!");

                // Cambiar estado a inactivo
                usuarioEncontrado.EsActivo = false;

                bool respuesta = await _usuarioRepository.Editar(usuarioEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("El usuario no se pudo desactivar!!!!");

                return respuesta; 
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> ActivarUsuario(int id)
        {
            var usuario = await _usuarioRepository.Obtener(u => u.IdUsuario == id);

            if (usuario == null)
                throw new TaskCanceledException("El usuario no existe.");

            usuario.EsActivo = true;

            bool actualizado = await _usuarioRepository.Editar(usuario);

            if (!actualizado)
                throw new TaskCanceledException("No se pudo activar el usuario.");

            return actualizado; //
        }

    }
}
