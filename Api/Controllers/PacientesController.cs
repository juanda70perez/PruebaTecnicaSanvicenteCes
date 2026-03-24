// <copyright file="PacientesController.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Api.Controllers
{
    using Domain.Dtos;
    using Domain.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// API controller that exposes endpoints for managing patients.
    /// All endpoints require a valid JWT token (simulating Azure AD protection).
    /// GET is accessible to both Admin and Viewer roles.
    /// POST and PUT require the Admin role.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PacientesController : ControllerBase
    {
        private readonly IPacienteService pacienteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PacientesController"/> class.
        /// </summary>
        /// <param name="pacienteService">The patient service injected via DI.</param>
        public PacientesController(IPacienteService pacienteService)
        {
            this.pacienteService = pacienteService;
        }

        /// <summary>
        /// Retrieves all active patients, optionally filtered by name or document number.
        /// Accessible to Admin and Viewer roles.
        /// </summary>
        /// <param name="filter">Optional search term to filter by name or document.</param>
        /// <param name="take">Optional limit for the number of records returned.</param>
        /// <returns>A list of <see cref="PacienteDto"/>.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Viewer")]
        [ProducesResponseType(typeof(IEnumerable<PacienteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerTodos([FromQuery] string? filter = null, [FromQuery] int? take = null)
        {
            var pacientes = await this.pacienteService.ObtenerTodosAsync(filter, take);
            return this.Ok(pacientes);
        }

        /// <summary>
        /// Registers a new patient. Returns 409 Conflict if an active patient with the same document already exists.
        /// Requires Admin role.
        /// </summary>
        /// <param name="pacienteDto">The patient data to register.</param>
        /// <returns>200 OK if successful, 409 Conflict if the document is already active.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Registrar([FromBody] PacienteDto pacienteDto)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var registrado = await this.pacienteService.RegistrarPacienteAsync(pacienteDto);

            if (!registrado)
            {
                return this.Conflict(new
                {
                    exito = false,
                    mensaje = "Ya existe un paciente activo con ese número de documento.",
                });
            }

            return this.Ok(new
            {
                exito = true,
                mensaje = "Paciente registrado correctamente.",
            });
        }

        /// <summary>
        /// Updates an existing patient's information by ID.
        /// The document number is not modifiable — only name, email, date of birth and active status can change.
        /// Requires Admin role.
        /// </summary>
        /// <param name="id">The ID of the patient to update.</param>
        /// <param name="actualizarDto">The updated patient data.</param>
        /// <returns>200 OK if successful, 404 Not Found if patient doesn't exist.</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarPacienteDto actualizarDto)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var actualizado = await this.pacienteService.ActualizarPacienteAsync(id, actualizarDto);

            if (!actualizado)
            {
                return this.NotFound(new
                {
                    exito = false,
                    mensaje = $"No se encontró un paciente con ID {id}.",
                });
            }

            return this.Ok(new
            {
                exito = true,
                mensaje = "Paciente actualizado correctamente.",
            });
        }
    }
}
