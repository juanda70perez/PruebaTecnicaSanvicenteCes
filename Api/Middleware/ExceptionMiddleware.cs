// <copyright file="PacienteService.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Api.Middleware
{
    using System.Net;
    using System.Text.Json;
    using Application.Commom;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// exception middleware is a custom middleware component designed to handle unhandled exceptions that occur during the processing of HTTP requests in an ASP.NET Core application. It captures exceptions, logs detailed information about the error, and returns a standardized JSON response to the client, ensuring that the application can gracefully handle errors and provide meaningful feedback to both developers and end-users.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly AppSettingsOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class with the specified dependencies, including the next middleware delegate, a logger for logging error information, and application settings options for configuring log file paths. This constructor sets up the necessary components for the middleware to function correctly within the ASP.NET Core request processing pipeline.
        /// </summary>
        /// <param name="next">next.</param>
        /// <param name="logger">logger.</param>
        /// <param name="options">options.</param>
        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IOptions<AppSettingsOptions> options)
        {
            this.next = next;
            this.logger = logger;
            this.options = options.Value;
        }

        /// <summary>
        /// Asynchronously invokes the middleware to handle HTTP requests, capturing any unhandled exceptions that occur during the processing of the request. If an exception is caught, it logs detailed information about the error and constructs a standardized JSON response to be sent back to the client, indicating that an error occurred and providing relevant details for debugging purposes. This method ensures that the application can gracefully handle errors and maintain a consistent response format for clients even in the event of unexpected issues.
        /// </summary>
        /// <param name="httpContext">httpContext.</param>
        /// <returns>task.</returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await this.next(httpContext);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Excepción capturada: {ex.Message}");

                await this.HandleExceptionAsync(httpContext, ex);
            }
        }

        /// <summary>
        /// Handles exceptions by logging detailed information about the error to a specified log file and constructing a standardized JSON response to be sent back to the client. The method attempts to write the error details, including the message, stack trace, and request path, to a log file defined in the application settings. If logging fails, it logs a critical error message. Finally, it creates a JSON response containing the success status, HTTP status code, user-friendly message, and technical details of the exception (which can be omitted in production for security reasons) and writes this response back to the client.
        /// </summary>
        /// <param name="context">context.</param>
        /// <param name="exception">exception.</param>
        /// <returns>task.</returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            try
            {
                string directory = Path.GetDirectoryName(this.options.LogPath) ?? "Logs";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]" +
                                    $"{Environment.NewLine}ERROR: {exception.Message}" +
                                    $"{Environment.NewLine}RUTA: {context.Request.Path}" +
                                    $"{Environment.NewLine}STACK: {exception.StackTrace}" +
                                    $"{Environment.NewLine}{new string('-', 50)}{Environment.NewLine}";

                await File.AppendAllTextAsync(this.options.LogPath, logMessage);
            }
            catch (Exception logEx)
            {
                this.logger.LogCritical($"Fallo crítico escribiendo el log en disco: {logEx.Message}");
            }

            var response = new
            {
                Exito = false,
                Codigo = context.Response.StatusCode,
                Mensaje = "Servicio no disponible. Intente más tarde.",
            };

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }
    }
}
