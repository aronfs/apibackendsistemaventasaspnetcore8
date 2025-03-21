using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;  

namespace SistemaVenta.DAL.Repositorios.Contrato
{
    public interface IGenericRepository<T> where T : class 
    {
        Task<T> Obtener(Expression<Func<T, bool>> filtro);
        Task<T> Crear(T entidad);
        Task<bool> Editar(T entidad);
        Task<bool> Eliminar(T entidad);

        Task<IQueryable<T>> Consultar(Expression<Func<T, bool>> filtro=null);
    }
}
