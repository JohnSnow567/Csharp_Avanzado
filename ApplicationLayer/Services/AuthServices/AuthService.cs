using CapaDominio.DTO;
using CapaDominio.Models;
using CapaInfraestructura.Repositorio.Auth;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CapaAplicacion.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        // Lista de usuarios en memoria para demostración
        private readonly List<Usuario> _usuarios = new List<Usuario>
        {
            new Usuario { Id = 1, Username = "admin", PasswordHash = "admin123", Roles = "Admin" },
            new Usuario { Id = 2, Username = "user", PasswordHash = "user123", Roles = "User" }
        };

        private readonly string _secretKey;

        public AuthService(string secretKey)
        {
            _secretKey = secretKey;
        }

        public LoginResponseDto Authenticate(LoginRequestDto loginRequest)
        {
            var usuario = _usuarios.FirstOrDefault(x =>
                x.Username.Equals(loginRequest.Username, StringComparison.InvariantCultureIgnoreCase) &&
                x.PasswordHash == loginRequest.Password);

            if (usuario == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Username),
                    new Claim(ClaimTypes.Role, usuario.Roles)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new LoginResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = tokenDescriptor.Expires.Value
            };
        }
    }
}
