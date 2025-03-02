using CapaDominio.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaInfraestructura.Repositorio.Auth
{
    public interface IAuthService
    {
        LoginResponseDto Authenticate(LoginRequestDto loginRequest);
    }
}
