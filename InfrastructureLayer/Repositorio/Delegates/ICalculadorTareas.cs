using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CapaInfraestructura.Repositorio.Delegates
{
    public delegate int CalcularDiasRestantesDelegate(Tareas tarea);

    public interface ICalculadorTareas
    {
        int CalcularDiasRestantes(CalcularDiasRestantesDelegate calcularDias, Tareas tarea);
    }
}
