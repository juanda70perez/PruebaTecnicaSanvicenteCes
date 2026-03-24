// <copyright file="IPacienteService.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Domain.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Domain.Dtos;

    /// <summary>
    /// Defines the contract for patient-related business operations, including registration with active status validation and functional search capabilities.
    /// </summary>
    public interface IPacienteService
    {
        /// <summary>
        /// Registers a new patient after validating that the patient is active. Returns true if registration is successful, otherwise false.
        /// </summary>
        /// <param name="pacienteDto">pacienteDto.</param>
        /// <returns>bool.</returns>
        Task<bool> RegistrarPacienteAsync(PacienteDto pacienteDto);

        /// <summary>
        /// Retrieves a list of patients based on an optional filter and a specified number of records to return. If no filter is provided, it returns all patients. If 'take' is specified, it limits the number of returned records accordingly.
        /// </summary>
        /// <param name="filter">filter.</param>
        /// <param name="take">take.</param>
        /// <returns>PacienteDto.</returns>
        Task<IEnumerable<PacienteDto>> ObtenerTodosAsync(string? filter = null, int? take = null);

        /// <summary>
        /// Updates an existing patient's information identified by their ID.
        /// Returns true if the update was successful, or false if no patient with that ID was found.
        /// </summary>
        /// <param name="id">The unique identifier of the patient to update.</param>
        /// <param name="actualizarDto">The new data to apply to the patient.</param>
        /// <returns>True if updated successfully; false if the patient was not found.</returns>
        Task<bool> ActualizarPacienteAsync(int id, ActualizarPacienteDto actualizarDto);
    }
}
