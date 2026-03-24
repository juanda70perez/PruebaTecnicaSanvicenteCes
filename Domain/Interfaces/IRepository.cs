// <copyright file="IRepository.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Domain.Interfaces
{
    using System.Linq.Expressions;

    /// <summary>
    /// Defines a generic repository interface for performing asynchronous CRUD operations and queries on entities of
    /// type T.
    /// </summary>
    /// <remarks>This interface abstracts data access logic, enabling decoupling of business logic from data
    /// storage concerns. Implementations typically interact with a database or other persistent storage. Methods are
    /// asynchronous to support non-blocking operations. Thread safety and transaction management depend on the specific
    /// implementation.</remarks>
    /// <typeparam name="T">The type of entity managed by the repository. Must be a reference type.</typeparam>
    public interface IRepository<T>
        where T : class
    {
        /// <summary>
        /// Asynchronously retrieves a collection of entities that match the specified criteria.
        /// </summary>
        /// <remarks>Use this method to perform flexible queries with optional filtering, ordering, and
        /// eager loading of related entities. The method executes asynchronously and is suitable for scenarios where
        /// non-blocking data access is required.</remarks>
        /// <param name="filter">An optional expression to filter the entities to be retrieved. If null, all entities are considered.</param>
        /// <param name="orderBy">An optional function to order the resulting entities. If null, the default ordering is used.</param>
        /// <param name="includeProperties">A comma-separated list of related entity property names to include in the query results. Specify an empty
        /// string to exclude related entities.</param>
        /// <param name="take">The maximum number of entities to return. If null, all matching entities are returned.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of
        /// entities that match the specified criteria.</returns>
        Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "",
        int? take = null);

        /// <summary>
        /// Asynchronously retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity of type T if found;
        /// otherwise, null.</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Asynchronously adds the specified entity to the data store.
        /// </summary>
        /// <param name="entity">The entity to add. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        Task AddAsync(T entity);

        /// <summary>
        /// Updates the specified entity in the data store.
        /// </summary>
        /// <param name="entity">The entity to update. Cannot be null. The entity must already exist in the data store.</param>
        void Update(T entity);

        /// <summary>
        /// Removes the specified entity from the collection.
        /// </summary>
        /// <param name="entity">The entity to remove from the collection. Cannot be null.</param>
        void Remove(T entity);
    }
}
