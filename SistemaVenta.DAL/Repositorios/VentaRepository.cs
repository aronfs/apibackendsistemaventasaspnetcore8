using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.Model.Models;

namespace SistemaVenta.DAL.Repositorios
{
    public class VentaRepository : GenericRepository <Venta>, IVentaRepository
    {
        private readonly DbventaContext _context;

        public VentaRepository(DbventaContext context):base(context)
        {
            _context = context;
        }

        public async Task<Venta> Registrar(Venta modelo)
        {
            Venta ventaGenerada = new Venta();  
            using(var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                   foreach(DetalleVenta dv in modelo.DetalleVenta)
                    {
                        Producto producto_encontrado = _context.Productos.Where(p=> p.IdProducto == dv.IdProducto).First();
                        producto_encontrado.Stock -= dv.Cantidad;
                        _context.Productos.Update(producto_encontrado);

                    }
                    await _context.SaveChangesAsync();
                    NumeroDocumento correlativo = _context.NumeroDocumentos.First();
                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaRegistro = DateTime.Now;
                    _context.NumeroDocumentos.Update(correlativo);
                    await _context.SaveChangesAsync();
                    int CantidadDigitos = 4;
                    string ceros = string.Concat(Enumerable.Repeat("0", CantidadDigitos));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                    //00001
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - CantidadDigitos, CantidadDigitos);

                    modelo.NumeroDocumento = numeroVenta;

                    await _context.Ventas.AddAsync(modelo);
                    await _context.SaveChangesAsync();

                    ventaGenerada = modelo;

                    transaction.Commit();

                }
                catch
                {
                   
                    transaction.Rollback();
                    throw;
                }

                return ventaGenerada;

            }
           
        }
    }
}
