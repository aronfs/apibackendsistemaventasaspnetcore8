using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using SistemaVenta.BLL.Services.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model.Models;

namespace SistemaVenta.BLL.Services
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IGenericRepository<Producto> _productoRepository;
        private readonly IMapper _mapper;

        public DashBoardService(IVentaRepository ventaRepository, IGenericRepository<Producto> productoRepository, IMapper mapper)
        {
            _ventaRepository = ventaRepository;
            _productoRepository = productoRepository;
            _mapper = mapper;
        }

        private IQueryable<Venta> retornarVentas(IQueryable<Venta> tablaVenta, int restarCantidadDias)
        {
            DateTime? ultimaFecha = tablaVenta.OrderByDescending(v => v.FechaRegistro)
                                              .Select(v => v.FechaRegistro)
                                              .FirstOrDefault();

            if (ultimaFecha == null)
            {
                Console.WriteLine("⚠ No hay ventas registradas en la base de datos.");
                return Enumerable.Empty<Venta>().AsQueryable(); // Retorna una consulta vacía
            }

            ultimaFecha = ultimaFecha.Value.AddDays(restarCantidadDias);

            Console.WriteLine($"📌 Filtrando ventas desde: {ultimaFecha.Value}");

            return tablaVenta.Where(v => v.FechaRegistro >= ultimaFecha);
        }

        private async Task<int> TotalVentasUltimaSemana()
        {
            IQueryable<Venta> queryVenta = await _ventaRepository.Consultar();

            Console.WriteLine($"📊 Total de ventas en BD: {queryVenta.Count()}");

            var tablaVenta = retornarVentas(queryVenta, -7);

            int totalVentas = tablaVenta.Count();

            Console.WriteLine($"📊 Total ventas última semana: {totalVentas}");

            return totalVentas;
        }

        private async Task<string> TotalIngresosUltimaSemana()
        {
            IQueryable<Venta> queryVenta = await _ventaRepository.Consultar();
            var tablaVenta = retornarVentas(queryVenta, -7);

            decimal resultado = tablaVenta.Any() ? tablaVenta.Sum(v => v.Total ?? 0) : 0;

            Console.WriteLine($"💰 Total ingresos última semana: {resultado}");

            return Convert.ToString(resultado, new CultureInfo("es-EC"));
        }

        private async Task<List<VentaSemanaDTO>> VentasUltimaSemana()
        {
            IQueryable<Venta> queryVenta = await _ventaRepository.Consultar();
            var tablaVenta = retornarVentas(queryVenta, -7);

            if (!tablaVenta.Any())
            {
                Console.WriteLine("📉 No hay ventas en la última semana.");
                return new List<VentaSemanaDTO>();
            }

            var ventasPorDia = tablaVenta.GroupBy(v => v.FechaRegistro.Value.Date)
                                         .OrderBy(v => v.Key)
                                         .Select(v => new VentaSemanaDTO
                                         {
                                             Fecha = v.Key.ToString("dd/MM/yyyy"),
                                             Total = v.Count()
                                         })
                                         .ToList();

            Console.WriteLine("📅 Ventas por día en la última semana:");
            foreach (var venta in ventasPorDia)
            {
                Console.WriteLine($"   - {venta.Fecha}: {venta.Total} ventas");
            }

            return ventasPorDia;
        }

        private async Task<int> TotalProductos()
        {
            var queryProducto = await _productoRepository.Consultar();

            if (queryProducto == null)
            {
                Console.WriteLine("⚠ No se encontraron productos en la base de datos.");
                return 0;
            }

            int totalProductos = queryProducto.Count();

            Console.WriteLine($"📦 Total de productos: {totalProductos}");

            return totalProductos;
        }

        public async Task<DashBoardDTO> Resumen()
        {
            DashBoardDTO vmDashBoard = new DashBoardDTO();
            try
            {
                Console.WriteLine("🔄 Generando resumen del dashboard...");

                vmDashBoard.TotalVentas = await TotalVentasUltimaSemana();
                vmDashBoard.TotalIngresos = await TotalIngresosUltimaSemana();
                vmDashBoard.TotalProductos = await TotalProductos();
                vmDashBoard.VentaUltimaSemana = await VentasUltimaSemana();

                Console.WriteLine("✅ Resumen generado con éxito.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en Resumen: {ex.Message}");
                throw;
            }
            return vmDashBoard;
        }
    }
}
