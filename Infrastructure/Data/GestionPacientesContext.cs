// <copyright file="GestionPacientesContext.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Infrastructure.Data
{
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// defines the database context for managing patient data, including configurations for the Paciente entity and its properties, as well as indices for optimized querying.
    /// </summary>
    public class GestionPacientesContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GestionPacientesContext"/> class using the specified options.
        /// </summary>
        /// <param name="options">option.</param>
        public GestionPacientesContext(DbContextOptions<GestionPacientesContext> options)
        : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the DbSet for Paciente entities, allowing for querying and saving instances of Paciente in the database.
        /// </summary>
        public DbSet<Paciente> Pacientes { get; set; }

        /// <summary>
        /// Configures the model by applying entity configurations for the Paciente entity, including property constraints and indices for optimized querying.
        /// </summary>
        /// <param name="modelBuilder">Modelbuilder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                      .ValueGeneratedOnAdd();

                entity.Property(p => p.NumeroDocumento)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(p => p.NombreCompleto)
                      .IsRequired()
                      .HasMaxLength(250);

                entity.Property(p => p.Email)
                      .IsRequired();

                entity.Property(p => p.FechaNacimiento)
                      .IsRequired();

                entity.Property(p => p.EstaActivo)
                      .IsRequired()
                      .HasDefaultValue(true);
            });

            this.CrearIndices(modelBuilder);
        }

        /// <summary>
        /// Create indices for the Paciente entity to optimize query performance on frequently searched fields such as NombreCompleto, Email, FechaNacimiento, and NumeroDocumento. This includes unique constraints where applicable to ensure data integrity.
        /// </summary>
        /// <param name="modelBuilder">Model Buider.</param>
        private void CrearIndices(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasIndex(p => p.NombreCompleto)
                      .HasDatabaseName("IX_Paciente_NombreCompleto");

                entity.HasIndex(p => p.Email)
                      .IsUnique()
                      .HasDatabaseName("IX_Paciente_Email");

                entity.HasIndex(p => p.FechaNacimiento)
                      .HasDatabaseName("IX_Paciente_FechaNacimiento");

                entity.HasIndex(p => p.NumeroDocumento)
                      .IsUnique()
                      .HasDatabaseName("IX_Paciente_NumeroDocumento");
            });
        }
    }
}
