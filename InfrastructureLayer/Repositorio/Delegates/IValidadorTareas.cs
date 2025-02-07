using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CapaInfraestructura.Repositorio.Delegates
{
    public delegate bool ValidarTareaDelegate(Tareas tarea);
    public interface IValidadorTareas
    {
        bool Validar(Tareas tarea);
    }
}
