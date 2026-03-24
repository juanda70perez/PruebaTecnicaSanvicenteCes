// <copyright file="PacienteServiceTest.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Test.ServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;
    using Application.Services;
    using Domain.Dtos;
    using Domain.Entities;
    using Domain.Interfaces;
    using Moq;

    /// <summary>
    /// test.
    /// </summary>
    public class PacienteServiceTest
    {
        /// <summary>
        /// Registrate a patient that already exists, should return false.
        /// </summary>
        /// <returns>false.</returns>
        [Fact]
        public async Task RegistrarPacienteAsync_PacienteYaExiste()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var pacienteExistente = new List<Paciente> { new Paciente { NumeroDocumento = "123", EstaActivo = true } };

            unitOfWorkMock.Setup(u => u.Pacientes.GetAsync(
                It.IsAny<Expression<Func<Paciente, bool>>>(),
                null,
                string.Empty,
                null))
                .ReturnsAsync(pacienteExistente);

            var service = new PacienteService(unitOfWorkMock.Object);
            var nuevoPacienteDto = new PacienteDto { Documento = "123", Nombre = "Juan" };

            var returnFalse = service.RegistrarPacienteAsync(nuevoPacienteDto);

            Assert.False(await returnFalse);
        }

        /// <summary>
        /// registrate a patient that does not exist, should return true.
        /// </summary>
        /// <returns>true.</returns>
        [Fact]
        public async Task Registrar_CuandoNoExiste_RetornaTrue()
        {
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Pacientes.GetAsync(It.IsAny<Expression<Func<Paciente, bool>>>(), null, string.Empty, null))
                   .ReturnsAsync(new List<Paciente>());
            uowMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

            var service = new PacienteService(uowMock.Object);
            var resultado = await service.RegistrarPacienteAsync(new PacienteDto { Documento = "123" });

            Assert.True(resultado);
        }

        /// <summary>
        /// search with text, should return mapped data and call the repository.
        /// </summary>
        /// <returns>true.</returns>
        [Fact]
        public async Task Buscar_ConTexto_DebeRetornarDatosMapeados_Y_LlamarAlRepositorio()
        {
            var uowMock = new Mock<IUnitOfWork>();

            var pacienteDb = new Paciente
            {
                Id = 1,
                NumeroDocumento = "102030",
                NombreCompleto = "Juan Perez",
                Email = "juan@mail.com",
                FechaNacimiento = new DateTime(2000, 1, 1),
                EstaActivo = true,
            };

            var listaFicticia = new List<Paciente> { pacienteDb };

            uowMock.Setup(u => u.Pacientes.GetAsync(
                It.IsAny<Expression<Func<Paciente, bool>>>(),
                It.IsAny<Func<IQueryable<Paciente>, IOrderedQueryable<Paciente>>>(),
                string.Empty,
                null))
                .ReturnsAsync(listaFicticia);

            var service = new PacienteService(uowMock.Object);
            string criterioBusqueda = "Juan";

            var resultado = await service.ObtenerTodosAsync(criterioBusqueda);

            Assert.NotNull(resultado);
            var pacienteResultado = resultado.First();
            Assert.Equal("Juan Perez", pacienteResultado.Nombre);
            Assert.Equal("102030", pacienteResultado.Documento);
            Assert.Equal(26, pacienteResultado.Edad);

            uowMock.Verify(
                u => u.Pacientes.GetAsync(
                It.IsAny<Expression<Func<Paciente, bool>>>(),
                It.IsAny<Func<IQueryable<Paciente>, IOrderedQueryable<Paciente>>>(),
                string.Empty,
                null),
                Times.Once);
        }

        /// <summary>
        /// get all patients, should return mapped data with the correct age calculated.
        /// </summary>
        /// <returns>true.</returns>
        [Fact]
        public async Task ObtenerTodosAsync_DebeMapearEdadCorrectamente()
        {
            var uowMock = new Mock<IUnitOfWork>();
            var fechaNac = new DateTime(2000, 1, 1);
            var pacientesDB = new List<Paciente> { new Paciente { FechaNacimiento = fechaNac, NumeroDocumento = "1" } };

            uowMock.Setup(u => u.Pacientes.GetAsync(It.IsAny<Expression<Func<Paciente, bool>>>(), It.IsAny<Func<IQueryable<Paciente>, IOrderedQueryable<Paciente>>>(), string.Empty, It.IsAny<int?>()))
                   .ReturnsAsync(pacientesDB);

            var service = new PacienteService(uowMock.Object);

            var resultado = await service.ObtenerTodosAsync();

            var edadCalculada = resultado.First().Edad;

            Assert.Equal(26, edadCalculada);
        }

        /// <summary>
        /// search with text, should call the repository with the correct filter.
        /// </summary>
        /// <param name="textoBusqueda">text to search.</param>
        /// <returns>true.</returns>
        [Theory]
        [InlineData("Juan Perez")]
        [InlineData("102030")]
        public async Task ObtenerTodosAsync_DebeLlamarAlRepositorio_ConFiltro(string textoBusqueda)
        {
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Pacientes.GetAsync(
                It.IsAny<Expression<Func<Paciente, bool>>>(),
                It.IsAny<Func<IQueryable<Paciente>, IOrderedQueryable<Paciente>>>(),
                string.Empty,
                null))
                .ReturnsAsync(new List<Paciente>());

            var service = new PacienteService(uowMock.Object);

            await service.ObtenerTodosAsync(filter: textoBusqueda);

            uowMock.Verify(
                u => u.Pacientes.GetAsync(
                It.IsAny<Expression<Func<Paciente, bool>>>(),
                It.IsAny<Func<IQueryable<Paciente>, IOrderedQueryable<Paciente>>>(),
                string.Empty,
                null), Times.Once);
        }

        /// <summary>
        /// get all patients with take, should return the correct number of records and call the repository with the correct take parameter.
        /// </summary>
        /// <returns>true.</returns>
        [Fact]
        public async Task ObtenerTodosAsync_DebeRetornarDatos_Y_PasarElLimiteTake()
        {
            var uowMock = new Mock<IUnitOfWork>();
            int cantidadEsperada = 2;

            var listaFicticia = new List<Paciente>
    {
        new Paciente { NumeroDocumento = "1", NombreCompleto = "Test 1", FechaNacimiento = DateTime.Now.AddYears(-20), Email = "Juan@hotmail.com"},
        new Paciente { NumeroDocumento = "2", NombreCompleto = "Test 2", FechaNacimiento = DateTime.Now.AddYears(-30), Email = "Juan2@hotmail.com" },
    };

            uowMock.Setup(u => u.Pacientes.GetAsync(
                It.IsAny<Expression<Func<Paciente, bool>>>(),
                It.IsAny<Func<IQueryable<Paciente>, IOrderedQueryable<Paciente>>>(),
                string.Empty,
                cantidadEsperada))
                .ReturnsAsync(listaFicticia);

            var service = new PacienteService(uowMock.Object);

            var resultado = await service.ObtenerTodosAsync(take: cantidadEsperada); 

            Assert.NotNull(resultado);
            Assert.Equal(cantidadEsperada, resultado.Count());
            Assert.Equal("Test 1", resultado.First().Nombre);

            uowMock.Verify(
                u => u.Pacientes.GetAsync(
                It.IsAny<Expression<Func<Paciente, bool>>>(),
                It.IsAny<Func<IQueryable<Paciente>, IOrderedQueryable<Paciente>>>(),
                string.Empty,
                cantidadEsperada), Times.Once);
        }
    }
}