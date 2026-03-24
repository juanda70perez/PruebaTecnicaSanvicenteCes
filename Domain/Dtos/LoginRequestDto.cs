// <copyright file="LoginRequestDto.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Domain.Dtos
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Data Transfer Object used to receive login credentials for JWT token generation.
    /// In production this would be validated against Azure AD or an identity store.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Gets or sets the username. Test users: "admin" or "viewer".
        /// </summary>
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 50 caracteres.")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password. Test passwords: "admin123" or "viewer123".
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener mínimo 6 caracteres.")]
        [StringLength(100, ErrorMessage = "La contraseña no puede superar los 100 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}
