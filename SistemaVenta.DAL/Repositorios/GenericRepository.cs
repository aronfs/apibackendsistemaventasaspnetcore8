using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace SistemaVenta.DAL.Repositorios
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbventaContext _context;

        public GenericRepository(DbventaContext context)
        {
            _context = context;
        }

        public async Task<T> Obtener(Expression<Func<T, bool>> filtro)
        {
            try
            {
                T entidad = await _context.Set<T>().FirstOrDefaultAsync(filtro);
                return entidad;
            }
            catch
            {
                throw;
            }
        }

        public async Task<T> Crear(T entidad)
        {
            try
            {
                _context.Set<T>().Add(entidad);
                await _context.SaveChangesAsync();
                return entidad;
            }
            catch
            {
                throw;
            }

        }

        public async Task<bool> Editar(T entidad)
        {
            try
            {
                _context.Set<T>().Update(entidad);  
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }

        }

        public async Task<bool> Eliminar(T entidad)
        {
            try
            {
                _context.Set<T>().Remove(entidad);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }

        }


        public async Task<IQueryable<T>> Consultar(Expression<Func<T, bool>> filtro = null)
        {
            try
            {
                IQueryable<T> query = filtro == null ? _context.Set<T>() : _context.Set<T>().Where(filtro);
                return query;
            }
            catch
            {
                throw;
            }

        }


    }

}
