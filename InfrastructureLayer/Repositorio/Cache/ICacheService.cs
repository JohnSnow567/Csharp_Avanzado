using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaInfraestructura.Repositorio.Cache
{
    public interface ICacheService
    {
        bool TryGetDiasRestantes(int id, out int diasRestantes);
        void SetDiasRestantes(int id, int diasRestantes);
        void RemoveTaskCache(int id);
    }
}
