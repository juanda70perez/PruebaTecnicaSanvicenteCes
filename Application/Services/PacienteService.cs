// <copyright file="PacienteService.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using Domain.Dtos;
    using Domain.Entities;
    using Domain.Interfaces;

    /// <summary>
    /// Provides business logic for managing patients, including registration with active status validation and functional search capabilities. This service interacts with the data layer through the Unit of Work pattern to ensure consistent data access and manipulation.
    /// </summary>
    public class PacienteService : IPacienteService
    {
        /// <summary>
        /// The unit of work for data access, allowing the service to perform operations on the patient repository and manage transactions effectively. This promotes a clean separation of concerns and ensures that all data operations are coordinated through a single interface.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="PacienteService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for data access.</param>
        public PacienteService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Registers a new patient after validating that there is no active patient with the same document number. If an active patient with the same document exists, the method returns false, indicating that the registration cannot proceed. Otherwise, it creates a new patient entity, adds it to the repository, and saves the changes through the unit of work, returning true if the operation was successful.
        /// </summary>
        /// <param name="pacienteDto">pacienteDto.</param>
        /// <returns>bool.</returns>
        public async Task<bool> RegistrarPacienteAsync(PacienteDto pacienteDto)
        {
            var existentes = await this.unitOfWork.Pacientes.GetAsync(
                filter: p => p.NumeroDocumento == pacienteDto.Documento && p.EstaActivo);

            if (existentes.Any())
            {
                return false;
            }

            var nuevoPaciente = new Paciente
            {
                NumeroDocumento = pacienteDto.Documento,
                NombreCompleto = pacienteDto.Nombre,
                Email = pacienteDto.Email,
                FechaNacimiento = pacienteDto.FechaNacimiento,
                EstaActivo = true,
            };

            await this.unitOfWork.Pacientes.AddAsync(nuevoPaciente);

            return await this.unitOfWork.SaveAsync() > 0;
        }

        /// <summary>
        /// Retrieves a list of patients based on an optional filter and a specified number of records to return. If no filter is provided, it returns all patients. If 'take' is specified, it limits the number of returned records accordingly.
        /// </summary>
        /// <param name="filter">filter.</param>
        /// <param name="take">take.</param>
        /// <returns>PacienteDto.</returns>
        public async Task<IEnumerable<PacienteDto>> ObtenerTodosAsync(string? filter = null, int? take = null)
        {
            var todos = await this.unitOfWork.Pacientes.GetAsync(
                orderBy: q => q.OrderByDescending(p => p.Id));

            IEnumerable<Paciente> resultado = todos;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var termino = Normalizar(filter);
                resultado = todos.Where(p =>
                    Normalizar(p.NombreCompleto).Contains(termino) ||
                    p.NumeroDocumento.Contains(termino));
            }

            // Aplicar límite en memoria después del filtro
            if (take.HasValue)
            {
                resultado = resultado.Take(take.Value);
            }

            return resultado.Select(p => new PacienteDto
            {
                Documento = p.NumeroDocumento,
                Nombre = p.NombreCompleto,
                Email = p.Email,
                FechaNacimiento = p.FechaNacimiento,
                Edad = this.CalcularEdad(p.FechaNacimiento),
            });
        }

        /// <summary>
        /// Updates an existing patient's editable information (name, email, date of birth, active status)
        /// by their ID. The document number is treated as an immutable identifier and is not modified.
        /// Returns false if no patient with the given ID exists.
        /// </summary>
        /// <param name="id">The unique identifier of the patient to update.</param>
        /// <param name="actualizarDto">The updated patient data.</param>
        /// <returns>True if the update was persisted successfully; false if the patient was not found.</returns>
        public async Task<bool> ActualizarPacienteAsync(int id, ActualizarPacienteDto actualizarDto)
        {
            var paciente = await this.unitOfWork.Pacientes.GetByIdAsync(id);

            if (paciente is null)
            {
                return false;
            }

            paciente.NombreCompleto = actualizarDto.Nombre;
            paciente.Email = actualizarDto.Email;
            paciente.FechaNacimiento = actualizarDto.FechaNacimiento;
            paciente.EstaActivo = actualizarDto.EstaActivo;

            this.unitOfWork.Pacientes.Update(paciente);

            return await this.unitOfWork.SaveAsync() > 0;
        }

        /// <summary>
        /// Normalizes a string by removing diacritical marks (tildes, accents) and converting to lowercase,
        /// so that "García", "garcia" and "GARCIA" all match the same search term.
        /// </summary>
        private static string Normalizar(string texto)
        {
            if (string.IsNullOrEmpty(texto))
            {
                return texto;
            }

            // Descompone cada carácter acentuado en su base + marca de acento (FormD)
            var descompuesto = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(descompuesto.Length);

            foreach (var c in descompuesto)
            {
                // Descarta las marcas de acento (NonSpacingMark) y conserva el carácter base
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC).ToLower();
        }

        /// <summary>
        /// Calculates age based on birth date.
        /// </summary>
        private int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - fechaNacimiento.Year;

            if (fechaNacimiento.Date > hoy.AddYears(-edad))
            {
                edad--;
            }

            return edad;
        }
    }
}
