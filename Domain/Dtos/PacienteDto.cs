// <copyright file="PacienteDto.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Domain.Dtos
{
    using System.ComponentModel.DataAnnotations;
    using Domain.Validators;

    /// <summary>
    /// Data Transfer Object used when registering a new patient.
    /// </summary>
    public class PacienteDto
    {
        /// <summary>
        /// Gets or sets the patient's identification document number (numeric, 5–15 digits).
        /// </summary>
        [Required(ErrorMessage = "El documento es obligatorio.")]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "El documento debe tener entre 5 y 15 caracteres.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El documento solo puede contener números.")]
        public string Documento { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the patient's full name.
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the patient's date of birth. Must be a past date.
        /// </summary>
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [FechaPasada]
        public DateTime FechaNacimiento { get; set; }

        /// <summary>
        /// Gets or sets the patient's calculated age (read-only, not required on input).
        /// </summary>
        public int Edad { get; set; } = 0;

        /// <summary>
        /// Gets or sets the patient's email address.
        /// </summary>
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
        public string Email { get; set; } = string.Empty;
    }
}
