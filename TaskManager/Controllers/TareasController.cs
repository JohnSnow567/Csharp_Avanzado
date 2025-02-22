using ApplicationLayer.Services.TaskServices;
using CapaAplicacion.Services.CacheServices;
using CapaInfraestructura.Repositorio.Delegates;
using DomainLayer.DTO;
using DomainLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    // Endpoints para la API de gestion de tareas
    public class TareasController : ControllerBase
    {
        private readonly TaskService _service;
        private readonly IValidadorTareas _validador;
        private readonly ICalculadorTareas _calculador;
        private readonly CacheService _cache;
        public TareasController(IValidadorTareas validador, 
            ICalculadorTareas calculador, 
            TaskService service,
            CacheService cache)
        {
            _validador = validador;
            _calculador = calculador;
            _service = service;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<Response<Tareas>>> GetTaskAllAsync()
            => await _service.GetTaskAllAsync();
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<Tareas>>> GetTaskByIdAllAsync(int id)
           => await _service.GetTaskByIdAllAsync(id);
        [HttpPost]
        public async Task<ActionResult<Response<string>>> AddTaskAllAsync(Tareas tarea)
        {
            
            if (!_validador.Validar(tarea))
            {
                return BadRequest("La tarea no es válida.");
            }

            
            var result = await _service.AddTaskAllAsync(tarea);

            return result;
        }
        [HttpPut]
        public async Task<ActionResult<Response<string>>> UpdateTaskAllAsync(Tareas tarea)
        {
            
            if (!_validador.Validar(tarea))
            {
                return BadRequest("La tarea no es válida.");
            }

            
            var result = await _service.UpdateTaskAllAsync(tarea);

            // Eliminamos el cache del diccionario ya que se actualizo la informacion de la tarea
            _cache.RemoveTaskCache(tarea.Id);

            return result;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<string>>> DeleteTaskAllAsync(int id)
        {
            var tarea = await _service.GetTaskByIdAllAsync(id);
            
            var result = await _service.DeleteTaskAllAsync(id);
            
            // Eliminamos el cache del diccionario ya que no lo necesitaremos mas
            _cache.RemoveTaskCache(id);

            return result;
        }

        [HttpGet("dias-restantes/{id}")]
        public async Task<ActionResult<Response<string>>> GetDiasRestantesAsync(int id)
        {
            // Hacemos una validacion para asegurarnos de que el cache no se repita
            if (_cache.TryGetDiasRestantes(id, out int cachedDiasRestantes))
            {
                return Ok(cachedDiasRestantes);
            }

            var tarea = await _service.GetTaskByIdAllAsync(id);

            if (tarea == null)
            {
                return NotFound("Tarea no encontrada.");
            }

            // Delegado para calcular los días restantes
            int diasRestantes = _calculador.CalcularDiasRestantes(
                t => (t.DueDate - DateTime.Now).Days, tarea.SingleData);

            // Agregamos al cache la cantidad que hemos seleccionado para optimizar memoria en futuras iteraciones de este endpoint
            _cache.SetDiasRestantes(id, diasRestantes);

            return Ok(diasRestantes);
        }

        [HttpPost("Crear Tarea Baja Prioridad")]
        public async Task<ActionResult<Response<string>>> AddLowPriorityTaskAsync(string descripcion)
        {

            var result = await _service.AddLowPriorityTask(descripcion);

            return result;
        }
        [HttpPost("Crear Tarea Alta Prioridad")]
        public async Task<ActionResult<Response<string>>> AddHighPriorityTaskAsync(string descripcion)
        {

            var result = await _service.AddHighPriorityTask(descripcion);

            return result;
        }

    }
}
