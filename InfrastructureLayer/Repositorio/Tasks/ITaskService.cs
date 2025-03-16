using DomainLayer.DTO;
using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaInfraestructura.Repositorio.Tasks
{
    public interface ITaskService
    {
        // Clase para implementar las pruebas unitarias con Mock
        Task<Response<Tareas>> GetTaskAllAsync();
        Task<Response<Tareas>> GetTaskByIdAllAsync(int id);
        Task<Response<string>> AddTaskAllAsync(Tareas tarea);
        Task<Response<string>> UpdateTaskAllAsync(Tareas tarea);
        Task<Response<string>> DeleteTaskAllAsync(int id);
        Task<Response<string>> AddLowPriorityTask(string descripcion);
        Task<Response<string>> AddHighPriorityTask(string descripcion);
    }
}
