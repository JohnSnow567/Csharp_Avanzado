using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Repositorio.Commons
{
    public interface ICommonsProcess<T> where T : class
    {
        // Procesos comunes para todas los servicios que requieran un CRUD basico
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetIdAsync(int id);
        Task<(bool IsSuccess, string Message)> AddAsync(T entry);
        Task<(bool IsSuccess, string Message)> UpdateAsync(T entry);
        Task<(bool IsSuccess, string Message)> DeleteAsync(int id);

    }
}
