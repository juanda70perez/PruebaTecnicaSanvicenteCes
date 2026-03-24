// <copyright file="IUnitOfWork.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Domain.Interfaces
{
    /// <summary>
    /// Defines a contract for coordinating changes across multiple repositories and persisting them as a single unit of
    /// work.
    /// </summary>
    /// <remarks>The unit of work pattern is commonly used to group related operations into a single
    /// transaction, ensuring consistency and simplifying data management. Implementations of this interface typically
    /// manage the lifecycle of repositories and provide a mechanism to commit or rollback changes as a single
    /// operation. This interface extends IDisposable, so it should be disposed when no longer needed to release
    /// resources such as database connections.</remarks>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the repository for accessing and managing patient entities.
        /// </summary>
        /// <remarks>Use this property to perform operations such as querying, adding, updating, or
        /// deleting patient records through the repository abstraction. The returned repository instance is typically
        /// used to encapsulate data access logic for patients.</remarks>
        IPacienteRepository Pacientes { get; }

        /// <summary>
        /// Asynchronously saves all pending changes to the underlying data store.
        /// </summary>
        /// <remarks>This method is typically used to persist changes made to tracked entities. The
        /// operation is performed asynchronously to avoid blocking the calling thread.</remarks>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries
        /// written to the data store.</returns>
        Task<int> SaveAsync();
    }
}
