using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SistemaVenta.BLL.Services.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model.Models;

namespace SistemaVenta.BLL.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IGenericRepository<Producto> _productoRepository;
        private readonly IMapper _mapper;

        public ProductoService(IGenericRepository<Producto> productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
        }

        public async Task<List<ProductoDTO>> Lista()
        {
            try
            {
                var queryProducto = await _productoRepository.Consultar();
                var listaProductos = queryProducto
                    .Include(cat => cat.IdCategoriaNavigation)
                    .Select(p => new Producto
                    {
                        IdProducto = p.IdProducto,
                        Nombre = p.Nombre,
                        IdCategoria = p.IdCategoria,
                        Stock = p.Stock,
                        Precio = p.Precio,
                        EsActivo = p.EsActivo,
                        FechaRegistro = p.FechaRegistro,
                        Foto = p.Foto
                    }).ToList();

                return _mapper.Map<List<ProductoDTO>>(listaProductos);
            }
            catch
            {
                throw;
            }
        }

        public async Task<ProductoDTO> Crear(ProductoDTO modelo)
        {
            try
            {
                var productoModelo = _mapper.Map<Producto>(modelo);

                //Guardar Foto directamente Url
                productoModelo.Foto = !string.IsNullOrEmpty(modelo.foto) ? modelo.foto : null;
                var productoCreado = await _productoRepository.Crear(productoModelo);

                if (productoCreado.IdProducto == 0)
                    throw new TaskCanceledException("No se pudo crear el producto");

                return _mapper.Map<ProductoDTO>(productoCreado);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Editar(ProductoDTO modelo)
        {
            try
            {
                var productoModelo = _mapper.Map<Producto>(modelo);
                var productoEncontrado = await _productoRepository.Obtener(x => x.IdProducto == productoModelo.IdProducto);

                if (productoEncontrado == null)
                    throw new TaskCanceledException("No se encontró el producto");

                productoEncontrado.Nombre = productoModelo.Nombre;
                productoEncontrado.IdCategoria = productoModelo.IdCategoria;
                productoEncontrado.Stock = productoModelo.Stock;
                productoEncontrado.Precio = productoModelo.Precio;
                productoEncontrado.EsActivo = productoModelo.EsActivo;

                productoEncontrado.Foto = !string.IsNullOrEmpty(productoEncontrado.Foto) ? productoModelo.Foto : productoEncontrado.Foto;
               

                bool respuesta = await _productoRepository.Editar(productoEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar el producto");

                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar1(int id)
        {
            try
            {
                var productoEncontrado = await _productoRepository.Obtener(x => x.IdProducto == id);

                if (productoEncontrado == null)
                    throw new TaskCanceledException("No se encontró el producto");

                bool respuesta = await _productoRepository.Eliminar(productoEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo eliminar el producto");
                return respuesta;
            }
            catch
            {
                throw;
            }
        }

      
    }
}
