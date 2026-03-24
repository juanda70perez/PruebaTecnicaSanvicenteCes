// <copyright file="ActualizarPacienteDto.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Domain.Dtos
{
    using System.ComponentModel.DataAnnotations;
    using Domain.Validators;

    /// <summary>
    /// Data Transfer Object used to update an existing patient's information.
    /// The document number is intentionally excluded — it acts as a stable identifier
    /// and should not be modified after registration.
    /// </summary>
    public class ActualizarPacienteDto
    {
        /// <summary>
        /// Gets or sets the patient's full name.
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the patient's email address.
        /// </summary>
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the patient's date of birth. Must be a past date.
        /// </summary>
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [FechaPasada]
        public DateTime FechaNacimiento { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the patient is active.
        /// Setting this to false effectively deactivates the patient record.
        /// </summary>
        public bool EstaActivo { get; set; } = true;
    }
}
