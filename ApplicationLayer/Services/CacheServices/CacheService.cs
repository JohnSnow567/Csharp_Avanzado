using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaAplicacion.Services.CacheServices
{
    public class CacheService
    {
        private readonly Dictionary<int, int> _diasRestantesCache = new Dictionary<int, int>();

        public bool TryGetDiasRestantes(int id, out int diasRestantes)
        {
            return _diasRestantesCache.TryGetValue(id, out diasRestantes);
        }

        public void SetDiasRestantes(int id, int diasRestantes)
        {
            _diasRestantesCache[id] = diasRestantes;
        }

        public void RemoveTaskCache(int id)
        {
            _diasRestantesCache.Remove(id);
        }
    }
}
