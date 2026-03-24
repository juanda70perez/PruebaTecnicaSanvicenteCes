// <copyright file="Paciente.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Domain.Entities
{
    /// <summary>
    /// Representa a un paciente con información personal y de contacto utilizada en el sistema de gestión de pacientes.
    /// </summary>
    public class Paciente
    {
        /// <summary>
        /// Gets or sets get.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets get.
        /// </summary>
        public string NumeroDocumento { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets get.
        /// </summary>
        public string NombreCompleto { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets get.
        /// </summary>
        public DateTime FechaNacimiento { get; set; }

        /// <summary>
        /// gets or sets get.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets get.
        /// </summary>
        public bool EstaActivo { get; set; } = true;
    }
}