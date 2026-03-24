// <copyright file="ServiceExtensions.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Api.Extensions
{
    using System.Text;
    using Api.Middleware;
    using Application.Commom;
    using Application.Services;
    using Domain.Interfaces;
    using Infrastructure.Data;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;

    /// <summary>
    /// Provides extension methods for configuring services in the application's dependency injection container,
    /// including CORS, JWT authentication, Swagger with Bearer support, and service registrations.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Configures Cross-Origin Resource Sharing (CORS) policies based on allowed origins in appsettings.
        /// </summary>
        public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = configuration.GetSection(AppSettingsOptions.SectionName)
                                           .Get<AppSettingsOptions>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    if (appSettings?.AllowedOrigins != null && appSettings.AllowedOrigins.Any())
                    {
                        builder.WithOrigins(appSettings.AllowedOrigins)
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    }
                    else
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                });
            });
        }

        /// <summary>
        /// Registers application services and repositories in the DI container.
        /// </summary>
        public static void ConfigureDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPacienteService, PacienteService>();
        }

        /// <summary>
        /// Adds the global exception handling middleware to the request pipeline.
        /// </summary>
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }

        /// <summary>
        /// Configures the SQLite database context using the connection string from configuration.
        /// </summary>
        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<GestionPacientesContext>(options =>
            {
                options.UseSqlite(connectionString);
                options.EnableSensitiveDataLogging();
            });
        }

        /// <summary>
        /// Configures JWT Bearer authentication using settings from the JwtSettings section in appsettings.
        /// Tokens are validated by issuer, audience, lifetime and signing key.
        /// In production, replace the local signing key with Azure AD's public key configuration.
        /// </summary>
        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero,
                };

                options.Events = new JwtBearerEvents
                {
                   
                    OnTokenValidated = context =>
                    {
                        var username = context.Principal?.Identity?.Name;
                        return Task.CompletedTask;
                    },

                    // context.Exception.Message te dice exactamente por qué falló
                    OnAuthenticationFailed = context =>
                    {
                        var error = context.Exception.Message;
                        return Task.CompletedTask;
                    },

                    // ← PON BREAKPOINT AQUÍ: se dispara cuando no hay token en el header
                    OnChallenge = context =>
                    {
                        var reason = context.ErrorDescription;
                        return Task.CompletedTask;
                    },
                };
            });

            services.AddAuthorization();
        }

        /// <summary>
        /// Configures Swagger with Bearer token support so endpoints can be tested
        /// directly from the Swagger UI using the JWT token obtained from POST /api/auth/token.
        /// </summary>
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Gestión de Pacientes",
                    Version = "v1",
                    Description = "Para autenticarte: primero llama a POST /api/auth/token con usuario/contraseña de prueba, " +
                                  "copia el token recibido y pégalo en el botón 'Authorize' con el formato: Bearer {token}",
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "Ingresa el token con el formato: Bearer {token}",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                        },
                        Array.Empty<string>()
                    },
                });

            });
        }
    }
}
