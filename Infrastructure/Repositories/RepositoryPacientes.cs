// <copyright file="RepositoryPacientes.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Infrastructure.Repositories
{
    using Domain.Entities;
    using Domain.Interfaces;
    using Infrastructure.Data;

    /// <summary>
    /// Provides a repository implementation for managing Paciente entities using the specified data context.
    /// </summary>
    /// <remarks>This class enables CRUD operations and query capabilities for Paciente entities by extending
    /// the generic Repository base class. It is intended to be used as the main data access point for Paciente-related
    /// persistence within the application.</remarks>
    public class RepositoryPacientes : Repository<Paciente>, IPacienteRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryPacientes"/> class with the specified database context, setting up the repository for managing Paciente entities.
        /// </summary>
        /// <param name="context">Context.</param>
        public RepositoryPacientes(GestionPacientesContext context)
            : base(context)
        {
        }
    }
}
