using Inventario.Domain.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inventario.WebAPI.Controllers
{
    /// <summary>
    /// Controlador de autenticación y operaciones de inicio de sesión.
    /// Proporciona generación de tokens JWT para acceso autorizado a endpoints protegidos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Autentica un usuario y retorna un token JWT para acceder a endpoints protegidos.
        /// </summary>
        /// <remarks>
        /// Use este token en el encabezado Authorization con el formato: Bearer {token}
        /// </remarks>
        /// <example>
        /// POST /api/auth/login
        /// {
        ///   "username": "admin",
        ///   "password": "yeniadmin"
        /// }
        /// 
        /// Response 200:
        /// {
        ///   "token": "eyJhbGciOiJIUzI1NiIs...",
        ///   "expiresIn": 3600,
        ///   "tokenType": "Bearer"
        /// }
        /// 
        /// Response 401:
        /// {
        ///   "message": "Credenciales inválidas"
        /// }
        /// </example>
        /// <param name="request">Credenciales de inicio de sesión</param>
        /// <returns>Token JWT si las credenciales son válidas</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var validUsername = _configuration["Auth:AdminUsername"] ?? "admin";
            var validPassword = _configuration["Auth:AdminPassword"];

            if (string.IsNullOrEmpty(validPassword))
            {
                return StatusCode(500, new { message = "Configuración de autenticación incompleta." });
            }

            if (request.Username != validUsername || request.Password != validPassword)
            {
                return Unauthorized(new UnauthorizedResponse { Message = "Credenciales inválidas" });
            }

            var token = GenerateJwtToken(request.Username);

            return Ok(new LoginResponse
            {
                Token = token,
                ExpiresIn = 3600,
                TokenType = "Bearer"
            });
        }

        private string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    /// <summary>
    /// Modelo de solicitud para inicio de sesión
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        /// <example>admin</example>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        /// <example>yeniadmin</example>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Modelo de respuesta con token JWT
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Token JWT generado para autenticación
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Tiempo de expiración del token en segundos
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Tipo de token (siempre Bearer)
        /// </summary>
        public string TokenType { get; set; } = "Bearer";
    }

    /// <summary>
    /// Modelo de respuesta para acceso no autorizado
    /// </summary>
    public class UnauthorizedResponse
    {
        /// <summary>
        /// Mensaje descriptivo del error de autenticación
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}