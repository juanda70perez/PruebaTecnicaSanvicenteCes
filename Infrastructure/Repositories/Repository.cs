// <copyright file="Repository.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;
    using Domain.Interfaces;
    using Infrastructure.Data;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides a generic repository implementation for performing common data access operations on entities of type T
    /// using Entity Framework Core.
    /// </summary>
    /// <remarks>This class implements the repository pattern to abstract data access logic for entities. It
    /// supports querying, adding, updating, and removing entities. The repository is intended to be used with a
    /// DbContext derived from GestionPacientesContext. Thread safety is not guaranteed; use a separate repository
    /// instance per unit of work or request.</remarks>
    /// <typeparam name="T">The entity type that the repository manages. Must be a reference type.</typeparam>
    public class Repository<T> : IRepository<T>
        where T : class
    {
        /// <summary>
        /// Provides access to the application's database context for patient management operations.
        /// </summary>
        /// <remarks>Intended for use by derived classes to perform data access and persistence tasks
        /// within the patient management domain.</remarks>
        private readonly GestionPacientesContext context;

        /// <summary>
        /// Represents the database set for the specified entity type used to perform CRUD operations within the
        /// context.
        /// </summary>
        /// <remarks>This field provides direct access to the underlying DbSet for advanced scenarios in
        /// derived repository classes. It is intended for use within subclasses and should not be exposed
        /// publicly.</remarks>
        private readonly DbSet<T> dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class with the specified database context, setting up.
        /// </summary>
        /// <param name="context">context.</param>
        public Repository(GestionPacientesContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        /// <summary>
        /// Asynchronously retrieves a collection of entities that match the specified filter, ordering, and included
        /// related properties.
        /// </summary>
        /// <remarks>This method supports filtering, ordering, eager loading of related entities, and
        /// limiting the number of results. The query is executed asynchronously against the underlying data
        /// source.</remarks>
        /// <param name="filter">An expression used to filter the entities to be returned. If null, all entities are included.</param>
        /// <param name="orderBy">A function to order the resulting entities. If null, the default ordering is used.</param>
        /// <param name="includeProperties">A comma-separated list of related entity property names to include in the query results. Use to eagerly load
        /// related data. If empty, no related entities are included.</param>
        /// <param name="take">The maximum number of entities to return. If null, all matching entities are returned.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of
        /// entities matching the specified criteria.</returns>
        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "",
            int? take = null)
        {
            IQueryable<T> query = this.dbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(
                new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Asynchronously retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity of type T if found;
        /// otherwise, null.</returns>
        public async Task<T> GetByIdAsync(int id) => await this.dbSet.FindAsync(id);

        /// <summary>
        /// Asynchronously adds the specified entity to the context for insertion into the database.
        /// </summary>
        /// <remarks>The entity is added to the context and will be inserted into the database when
        /// SaveChangesAsync is called. This method does not immediately persist changes to the database.</remarks>
        /// <param name="entity">The entity to add. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        public async Task AddAsync(T entity) => await this.dbSet.AddAsync(entity);

        /// <summary>
        /// Updates the specified entity in the underlying data set.
        /// </summary>
        /// <remarks>The entity must already be tracked by the context. Changes will be persisted to the
        /// database when SaveChanges is called.</remarks>
        /// <param name="entity">The entity to update. Cannot be null.</param>
        public void Update(T entity) => this.dbSet.Update(entity);

        /// <summary>
        /// Removes the specified entity from the underlying data set.
        /// </summary>
        /// <remarks>The entity is marked for deletion and will be removed from the data store when
        /// changes are saved. If the entity is not being tracked, this method has no effect.</remarks>
        /// <param name="entity">The entity to remove from the data set. Cannot be null.</param>
        public void Remove(T entity) => this.dbSet.Remove(entity);
    }
}