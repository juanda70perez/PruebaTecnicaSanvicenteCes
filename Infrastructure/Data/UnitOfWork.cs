// <copyright file="UnitOfWork.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Infrastructure.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Domain.Entities;
    using Domain.Interfaces;
    using Infrastructure.Repositories;

    /// <summary>
    /// Coordinates the work of multiple repositories and manages the persistence of changes as a single unit of work
    /// within the application's data context.
    /// </summary>
    /// <remarks>Use this class to group related repository operations and ensure that all changes are
    /// committed together. This pattern helps maintain data consistency and simplifies transaction management. The
    /// instance should be disposed after use to release database resources.</remarks>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GestionPacientesContext context;

        private IPacienteRepository pacientes;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class using the specified database context.
        /// </summary>
        /// <param name="context">The database context to be used for data operations. Cannot be null.</param>
        public UnitOfWork(GestionPacientesContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the repository for accessing and managing patient entities.
        /// </summary>
        public IPacienteRepository Pacientes => this.pacientes ??= new RepositoryPacientes(this.context);

        /// <summary>
        /// Saves all changes made in the context to the underlying database asynchronously, ensuring that all operations.
        /// </summary>
        /// <returns>result.</returns>
        public async Task<int> SaveAsync() => await this.context.SaveChangesAsync();

        /// <summary>
        /// Releases all resources used by the current instance of the class.
        /// </summary>
        /// <remarks>Call this method when the instance is no longer needed to free unmanaged resources
        /// promptly. After calling this method, the instance should not be used.</remarks>
        public void Dispose() => this.context.Dispose();
    }
}
