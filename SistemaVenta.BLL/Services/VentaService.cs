using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IGenericRepository<DetalleVenta> _detalleVentaRepository;
        private readonly IMapper _mapper;

        public VentaService(IVentaRepository ventaRepository, IGenericRepository<DetalleVenta> detalleVentaRepository, IMapper mapper)
        {
            _ventaRepository = ventaRepository;
            _detalleVentaRepository = detalleVentaRepository;
            _mapper = mapper;
        }
        public async Task<VentaDTO> Registrar(VentaDTO venta)
        {
            try
            {
                var ventaGenerada = await _ventaRepository.Registrar(_mapper.Map<Venta>(venta));
                if (ventaGenerada.IdVenta == 0)
                    throw new TaskCanceledException("No se pudo registrar la venta");
                return _mapper.Map<VentaDTO>(ventaGenerada);
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<VentaDTO>> Historial(string buscarPro, string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> queryVenta = await _ventaRepository.Consultar();
            var listaResultado = new List<Venta>();
            try
            {
                if (buscarPro == "fecha")
                {
                    DateTime fechaInicioConvertida = DateTime.SpecifyKind(
                        DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-EC")),
                        DateTimeKind.Utc);

                    DateTime fechaFinConvertida = DateTime.SpecifyKind(
                        DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-EC")),
                        DateTimeKind.Utc);

                    listaResultado = await queryVenta.Where(
                        v =>
                        v.FechaRegistro.Value.Date >= fechaInicioConvertida.Date &&
                        v.FechaRegistro.Value.Date <= fechaFinConvertida.Date)
                        .Include(dv => dv.DetalleVenta)
                        .ThenInclude(p => p.IdProductoNavigation)
                        .ToListAsync();
                }
                else
                {
                    listaResultado = await queryVenta.Where(
                      v => v.NumeroDocumento == numeroVenta)
                      .Include(dv => dv.DetalleVenta)
                      .ThenInclude(p => p.IdProductoNavigation)
                      .ToListAsync();
                }
            }
            catch
            {
                throw;
            }
            return _mapper.Map<List<VentaDTO>>(listaResultado);
        }



        public async Task<List<ReporteDTO>> Reporte(string fechaInicio, string fechaFin)
        {
            IQueryable<DetalleVenta> queryVenta = await _detalleVentaRepository.Consultar();
            var ListaResultado = new List<DetalleVenta>();

            try
            {
                DateTime fechaInicioConvertida = DateTime.SpecifyKind(
                    DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-EC")),
                    DateTimeKind.Utc);

                DateTime fechaFinConvertida = DateTime.SpecifyKind(
                    DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-EC")),
                    DateTimeKind.Utc);

                ListaResultado = await queryVenta
                    .Include(p => p.IdProductoNavigation)
                    .Include(v => v.IdVentaNavigation)
                    .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= fechaInicioConvertida.Date &&
                                 dv.IdVentaNavigation.FechaRegistro.Value.Date <= fechaFinConvertida.Date)
                    .ToListAsync();
            }
            catch
            {
                throw;
            }

            return _mapper.Map<List<ReporteDTO>>(ListaResultado);
        }
    }
}
