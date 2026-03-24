// <copyright file="FechaPasadaAttribute.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Domain.Validators
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Custom validation attribute that ensures a DateTime value is strictly in the past.
    /// Also enforces a minimum year of 1900 and a maximum age of 150 years.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class FechaPasadaAttribute : ValidationAttribute
    {
        private const int EdadMaximaAnios = 150;
        private const int AnioMinimo = 1900;

        /// <summary>
        /// Initializes a new instance of the <see cref="FechaPasadaAttribute"/> class.
        /// </summary>
        public FechaPasadaAttribute()
            : base("La fecha de nacimiento debe ser una fecha pasada válida.")
        {
        }

        /// <inheritdoc />
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return new ValidationResult("La fecha de nacimiento es obligatoria.");
            }

            if (value is not DateTime fecha)
            {
                return new ValidationResult("Valor de fecha inválido.");
            }

            if (fecha.Year < AnioMinimo)
            {
                return new ValidationResult($"El año no puede ser anterior a {AnioMinimo}.");
            }

            if (fecha >= DateTime.Today)
            {
                return new ValidationResult("La fecha de nacimiento debe ser anterior a hoy.");
            }

            if (DateTime.Today.Year - fecha.Year > EdadMaximaAnios)
            {
                return new ValidationResult($"La edad no puede superar los {EdadMaximaAnios} años.");
            }

            return ValidationResult.Success;
        }
    }
}
