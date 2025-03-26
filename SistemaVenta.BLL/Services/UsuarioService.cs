﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                var listaUsuario = queryUsuario.Include(rol => rol.IdRolNavigation)
                    .Select(u => new Usuario
                    {
                        IdUsuario = u.IdUsuario,
                        NombreCompleto = u.NombreCompleto,
                        Correo = u.Correo,
                        IdRol = u.IdRol,
                        Clave = u.Clave,
                        EsActivo = u.EsActivo,
                        FechaRegistro = u.FechaRegistro,
                        Foto = u.Foto
                    }).ToList();

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
                var usuarioCrear = _mapper.Map<Usuario>(modelo);

                // Guardar directamente la URL de la imagen
                usuarioCrear.Foto = !string.IsNullOrEmpty(modelo.Foto) ? modelo.Foto : null;

                var usuarioCreado = await _usuarioRepository.Crear(usuarioCrear);

                if (usuarioCreado.IdUsuario == 0)
                    throw new TaskCanceledException("El usuario no se pudo crear!!!!");

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
                var usuarioModelo = _mapper.Map<Usuario>(modelo);
                var usuarioEncontrado = await _usuarioRepository.Obtener(u => u.IdUsuario == usuarioModelo.IdUsuario);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no Existe!!!!");

                usuarioEncontrado.NombreCompleto = usuarioModelo.NombreCompleto;
                usuarioEncontrado.Correo = usuarioModelo.Correo;
                usuarioEncontrado.IdRol = usuarioModelo.IdRol;
                usuarioEncontrado.Clave = usuarioModelo.Clave;
                usuarioEncontrado.EsActivo = usuarioModelo.EsActivo;
                // Guardar la URL en la base de datos
                usuarioEncontrado.Foto = !string.IsNullOrEmpty(usuarioModelo.Foto) ? usuarioModelo.Foto : usuarioEncontrado.Foto;


                bool respuesta = await _usuarioRepository.Editar(usuarioEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("El usuario no se pudo editar!!!!");

                return respuesta;
            }
            catch
            {
                throw;
            }
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
                    throw new TaskCanceledException("El usuario no se pudo eliminar!!!!");

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
                return usuario?.Foto;  // Retorna la URL directamente
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la foto del usuario", ex);
            }
        }

       
    }
}
