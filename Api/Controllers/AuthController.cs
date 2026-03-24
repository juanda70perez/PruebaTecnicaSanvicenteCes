// <copyright file="AuthController.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Api.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Domain.Dtos;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Handles authentication and JWT token generation.
    /// In production this controller would delegate to Azure AD via OAuth 2.0.
    /// For this demo, it validates hardcoded test users and returns a signed JWT.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;

        // Usuarios de prueba: simulan lo que vendría de Azure AD o una base de datos de identidades.
        // Rol "Admin" puede crear y actualizar pacientes.
        // Rol "Viewer" solo puede consultar.
        private static readonly Dictionary<string, (string Password, string Rol)> UsuariosPrueba = new()
        {
            { "admin",  ("admin123",  "Admin") },
            { "viewer", ("viewer123", "Viewer") },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="configuration">App configuration for reading JWT settings.</param>
        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Generates a JWT Bearer token for valid credentials.
        /// Use the returned token en el botón "Authorize" de Swagger con el formato: Bearer {token}.
        /// Usuarios de prueba — admin: admin123 | viewer: viewer123.
        /// </summary>
        /// <param name="loginDto">Credentials.</param>
        /// <returns>JWT token with expiration date, or 401 if credentials are invalid.</returns>
        [HttpPost("token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GenerarToken([FromBody] LoginRequestDto loginDto)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (!UsuariosPrueba.TryGetValue(loginDto.Username.ToLower(), out var userData)
                || userData.Password != loginDto.Password)
            {
                return this.Unauthorized(new
                {
                    exito = false,
                    mensaje = "Usuario o contraseña incorrectos.",
                });
            }

            var token = this.GenerarJwt(loginDto.Username, userData.Rol);
            var expira = DateTime.UtcNow.AddMinutes(
                int.Parse(this.configuration["JwtSettings:ExpirationMinutes"] ?? "60"));

            return this.Ok(new
            {
                exito = true,
                token,
                rol = userData.Rol,
                expira,
                uso = $"Copia el token y en Swagger presiona 'Authorize', escribe: Bearer {token[..20]}...",
            });
        }

        private string GenerarJwt(string username, string rol)
        {
            var jwtSettings = this.configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, rol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64),
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(jwtSettings["ExpirationMinutes"] ?? "60")),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
