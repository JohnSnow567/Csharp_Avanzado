using CapaInfraestructura.Repositorio.Delegates;
using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaAplicacion.Services.DelegateServices
{
    public class ValidadorTareas : IValidadorTareas
    {
        private readonly ValidarTareaDelegate _validador;

        public ValidadorTareas()
        {
            _validador = ValidarDescripcionNoVacia;
            _validador += ValidarFechaVencimiento;
        }

        public bool Validar(Tareas tarea)
        {
            foreach (ValidarTareaDelegate validador in _validador.GetInvocationList())
            {
                if (!validador(tarea))
                {
                    return false;
                }
            }
            return true;
        }

        private bool ValidarDescripcionNoVacia(Tareas tarea) => !string.IsNullOrEmpty(tarea.Description);
        private bool ValidarFechaVencimiento(Tareas tarea) => tarea.DueDate > DateTime.Now;
    }
}
