// <copyright file="IPacienteRepository.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Domain.Interfaces
{
    using Domain.Entities;

    /// <summary>
    /// Defines a contract for repository operations specific to the Paciente entity.
    /// </summary>
    /// <remarks>This interface extends to provide a standardized set of data access
    /// methods for Paciente objects. Implementations should ensure thread safety if used in multi-threaded
    /// scenarios.</remarks>
    public interface IPacienteRepository : IRepository<Paciente>
    {
    }
}
