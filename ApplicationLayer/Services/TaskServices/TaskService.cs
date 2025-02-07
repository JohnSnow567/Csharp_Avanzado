using DomainLayer.DTO;
using DomainLayer.Models;
using InfrastructureLayer.Repositorio.Commons;
using CapaInfraestructura.Repositorio.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Services.TaskServices
{
    // Servicio del manejo de tareas
    public class TaskService
    {
        private readonly ITask _tareasProcess;

        public TaskService(ITask tareaProcess)
        {
            _tareasProcess = tareaProcess;
        }

        // Metodo para conseguir todas las tareas
        public async Task<Response<Tareas>> GetTaskAllAsync()
        {
            var response = new Response<Tareas>();
            try
            {
                response.DataList = await _tareasProcess.GetAllAsync();
                response.Successful = true;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

        // Metodo para conseguir una tarea por el ID
        public async Task<Response<Tareas>> GetTaskByIdAllAsync(int id)
        {
            var response = new Response<Tareas>();
            try
            {
                var result = await _tareasProcess.GetIdAsync(id);
                if (result != null) { 
                    response.SingleData = result;
                    response.Successful = true;
                }
                else
                {
                    response.Successful = false;
                    response.Message = "No se encontro información...";
                }
               
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

        // Metodo para agregar tareas
        public async Task<Response<string>> AddTaskAllAsync(Tareas tarea)
        {
            var response = new Response<string>();
            try
            {

                var result = await _tareasProcess.AddAsync(tarea);
                response.Message = result.Message;
                response.Successful = result.IsSuccess;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

        // Metodo para actualizar tareas
        public async Task<Response<string>> UpdateTaskAllAsync(Tareas tarea)
        {
            var response = new Response<string>();
            try
            {
                var result = await _tareasProcess.UpdateAsync(tarea);
                response.Message = result.Message;
                response.Successful = result.IsSuccess;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

        // Metodo para borrar tareas
        public async Task<Response<string>> DeleteTaskAllAsync(int id)
        {
            var response = new Response<string>();
            try
            {
                var result = await _tareasProcess.DeleteAsync(id);
                response.Message = result.Message;
                response.Successful = result.IsSuccess;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

        public async Task<Response<string>> AddLowPriorityTask(string descripcion)
        {
            var response = new Response<string>();
            try
            {

                var result = await _tareasProcess.AddLowPriorityTask(descripcion);
                response.Message = result.Message;
                response.Successful = result.IsSuccess;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

        public async Task<Response<string>> AddHighPriorityTask(string descripcion)
        {
            var response = new Response<string>();
            try
            {

                var result = await _tareasProcess.AddHighPriorityTask(descripcion);
                response.Message = result.Message;
                response.Successful = result.IsSuccess;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }
    }
}
