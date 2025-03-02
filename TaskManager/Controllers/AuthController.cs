using CapaDominio.DTO;
using CapaInfraestructura.Repositorio.Auth;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto loginRequest)
        {
            var result = _authService.Authenticate(loginRequest);
            if (result == null)
                return Unauthorized("Credenciales inválidas");

            return Ok(result);
        }
    }
}
