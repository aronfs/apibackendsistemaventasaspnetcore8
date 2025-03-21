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
    public class MenuService: IMenuService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly IGenericRepository<MenuRol> _menuRolRepository;
        private readonly IGenericRepository<Menu> _menuRepository;
        private readonly IMapper _mapper;

        public MenuService(IGenericRepository<Usuario> usuarioRepository, IGenericRepository<MenuRol> menuRolRepository, IGenericRepository<Menu> menuRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _menuRolRepository = menuRolRepository;
            _menuRepository = menuRepository;
            _mapper = mapper;
        }

        public async Task<List<MenuDTO>> Listar(int idUsuario)
        {
            IQueryable<Usuario> tbUsuario = await _usuarioRepository.Consultar(u => u.IdUsuario == idUsuario);
            IQueryable<MenuRol> tbMenuRol = await _menuRolRepository.Consultar();
            IQueryable<Menu> tbMenu = await _menuRepository.Consultar();

            Console.WriteLine($"Usuarios encontrados: {tbUsuario.Count()}");
            Console.WriteLine($"Menús por rol encontrados: {tbMenuRol.Count()}");
            Console.WriteLine($"Total de menús: {tbMenu.Count()}");

            try
            {
                IQueryable<Menu> tbResultado = (from u in tbUsuario
                                                join mr in tbMenuRol on u.IdRol equals mr.IdRol
                                                join mn in tbMenu on mr.IdMenu equals mn.IdMenu
                                                select mn).AsQueryable();

                Console.WriteLine($"Menús encontrados para usuario {idUsuario}: {tbResultado.Count()}");

                var listaMenus = tbResultado.ToList();

                return _mapper.Map<List<MenuDTO>>(listaMenus);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Listar: {ex.Message}");
                throw;
            }
        }

    }
}
