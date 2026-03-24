// <copyright file="DataSeeder.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Infrastructure.Data
{
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Seeds the database with 100 sample patients when it is empty.
    /// Safe to call multiple times — only runs if no patients exist yet.
    /// </summary>
    public static class DataSeeder
    {
        /// <summary>
        /// Seeds 100 patients into the database if the Pacientes table is empty.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public static async Task SeedAsync(GestionPacientesContext context)
        {
            if (await context.Pacientes.AnyAsync())
            {
                return;
            }

            var pacientes = GenerarPacientes();
            await context.Pacientes.AddRangeAsync(pacientes);
            await context.SaveChangesAsync();
        }

        private static List<Paciente> GenerarPacientes()
        {
            var nombres = new[]
            {
                "Santiago", "Valentina", "Sebastián", "Isabella", "Mateo",
                "Sofía", "Nicolás", "Camila", "Samuel", "Luciana",
                "Andrés", "Mariana", "Felipe", "Daniela", "Diego",
                "Paula", "Alejandro", "Juliana", "Miguel", "Natalia",
                "Esteban", "Laura", "David", "Sara", "Juan",
                "María", "Jorge", "Ana", "Carlos", "Claudia",
                "Álvaro", "Patricia", "Hernán", "Gloria", "Rodrigo",
                "Diana", "Germán", "Mónica", "Arturo", "Ximena",
            };

            var apellidos = new[]
            {
                "García", "Rodríguez", "Martínez", "López", "González",
                "Hernández", "Pérez", "Sánchez", "Ramírez", "Torres",
                "Flores", "Rivera", "Gómez", "Díaz", "Reyes",
                "Cruz", "Morales", "Ortiz", "Gutiérrez", "Vargas",
                "Castillo", "Jiménez", "Moreno", "Ramos", "Romero",
                "Suárez", "Medina", "Aguilar", "Castro", "Chávez",
                "Patiño", "Ríos", "Cano", "Ospina", "Cardona",
                "Zapata", "Mejía", "Salazar", "Montoya", "Acosta",
                "Quintero", "Vélez", "Muñoz", "Córdoba", "Herrera",
            };

            var dominios = new[]
            {
                "gmail.com", "hotmail.com", "yahoo.com",
                "outlook.com", "live.com",
            };

            var rng = new Random(42);
            var pacientes = new List<Paciente>(100);
            var documentosUsados = new HashSet<string>();
            var emailsUsados = new HashSet<string>();

            for (int i = 1; i <= 100; i++)
            {
                var nombre = nombres[rng.Next(nombres.Length)];
                var apellido1 = apellidos[rng.Next(apellidos.Length)];
                var apellido2 = apellidos[rng.Next(apellidos.Length)];
                var nombreCompleto = $"{nombre} {apellido1} {apellido2}";

                string documento;
                do
                {
                    var longitud = rng.Next(8, 11); // 8, 9 o 10 dígitos
                    var min = (long)Math.Pow(10, longitud - 1);
                    var max = (long)Math.Pow(10, longitud) - 1;
                    documento = rng.NextInt64(min, max).ToString();
                }
                while (documentosUsados.Contains(documento));
                documentosUsados.Add(documento);

                string email;
                var baseEmail = QuitarTildes($"{nombre.ToLower()}.{apellido1.ToLower()}{i}");
                do
                {
                    var dominio = dominios[rng.Next(dominios.Length)];
                    email = $"{baseEmail}@{dominio}";
                }
                while (emailsUsados.Contains(email));
                emailsUsados.Add(email);

                var diasAtras = rng.Next(365, 365 * 80);
                var fechaNacimiento = DateTime.Today.AddDays(-diasAtras);

                var estaActivo = rng.Next(10) != 0;

                pacientes.Add(new Paciente
                {
                    NumeroDocumento = documento,
                    NombreCompleto = nombreCompleto,
                    Email = email,
                    FechaNacimiento = fechaNacimiento,
                    EstaActivo = estaActivo,
                });
            }

            return pacientes;
        }

        /// <summary>
        /// Removes accent marks so email addresses are ASCII-safe.
        /// </summary>
        private static string QuitarTildes(string texto)
        {
            return texto
                .Replace("á", "a").Replace("é", "e").Replace("í", "i")
                .Replace("ó", "o").Replace("ú", "u").Replace("ü", "u")
                .Replace("ñ", "n").Replace("Á", "a").Replace("É", "e")
                .Replace("Í", "i").Replace("Ó", "o").Replace("Ú", "u")
                .Replace("Ñ", "n");
        }
    }
}
