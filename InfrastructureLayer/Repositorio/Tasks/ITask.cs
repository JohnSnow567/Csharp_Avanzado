using DomainLayer.Models;
using InfrastructureLayer.Repositorio.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaInfraestructura.Repositorio.Tasks
{
    // Interfaz nueva para extender el dominio de manejo de tareas mas alla de los metodos comunes
    public interface ITask : ICommonsProcess<Tareas>
    {
        Task<(bool IsSuccess, string Message)> AddLowPriorityTask(string Descripcion);
        Task<(bool IsSuccess, string Message)> AddHighPriorityTask(string Descripcion);

    }
}
