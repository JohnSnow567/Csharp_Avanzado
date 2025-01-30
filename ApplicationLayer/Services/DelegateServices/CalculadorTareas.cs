using CapaInfraestructura.Repositorio.Delegates;
using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CapaAplicacion.Services.DelegateServices
{
    public class CalculadorTareas : ICalculadorTareas
    {
        public int CalcularDiasRestantes(CalcularDiasRestantesDelegate calcularDias, Tareas tarea)
        {
            return calcularDias(tarea);
        }
    }
}
