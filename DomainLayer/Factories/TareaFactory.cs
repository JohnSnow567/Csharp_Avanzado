using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDominio.Factories
{
    // Fabrica delegada para la creacion de tareas de baja o alta prioridad
    public static class TareaFactory
    {
        public static Tareas CrearTareaAltaPrioridad(string Descripcion)
        {
            return new Tareas
            {
                Id = 0,
                Description = Descripcion,
                AdditionalData = " Prioridad Alta",
                DueDate = DateTime.Now.AddDays(1),
                Status = "Pending"
            };
        }

        public static Tareas CrearTareaBajaPrioridad(string Descripcion)
        {
            return new Tareas
            {
                Id = 0,
                Description = Descripcion,
                AdditionalData = " Prioridad Baja",
                DueDate = DateTime.Now.AddDays(7),
                Status = "Pending"
            };
        }

    }
}
