using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.Identity.Client;
using SistemaVenta.BLL.Services.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model.Models;

namespace SistemaVenta.BLL.Services
{
    public class RolService : IRolService
    {
        private readonly IGenericRepository<Rol> _rolRepository;
        private readonly IMapper _mapper;

        public RolService(IGenericRepository<Rol> rolRepository, IMapper mapper)
        {
            _rolRepository = rolRepository;
            _mapper = mapper;
        }

        public async Task<List<RolDTO>> Lista()
        {
            try { 
                var listaRoles = await _rolRepository.Consultar();
                return _mapper.Map<List<RolDTO>>(listaRoles.ToList());
            } catch
            {
                throw;
            }
        }

        public async Task<string?> ObtenerRolNombre(string idRol)
        {


            try
            {
                var rol = await _rolRepository.Consultar(r => r.IdRol.ToString() == idRol);

                // Si se encuentra el rol, devolver su descripción; si no, devolver null
                return rol.FirstOrDefault()?.Nombre;
            }
            catch
            {
                return null;
            }

        }
}
}
