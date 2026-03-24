// <copyright file="AppSettingsOptions.cs" company="SanVicenteCes">
// Copyright (c) SanVicenteCes. All rights reserved.
// </copyright>

namespace Application.Commom
{
    using System;

    /// <summary>
    /// Represents the application settings options that can be configured in the appsettings.json file, providing a strongly-typed class for accessing configuration values related to allowed CORS origins and log file paths within the application.
    /// </summary>
    public class AppSettingsOptions
    {
        /// <summary>
        /// Defines the name of the configuration section in the appsettings.json file that corresponds to this class, allowing for easy mapping of configuration values to strongly-typed properties within the application.
        /// </summary>
        public const string SectionName = "AppSettings";

        /// <summary>
        /// Gets or sets an array of allowed origins for Cross-Origin Resource Sharing (CORS) policies, specifying which client applications are permitted to access the API resources based on their origin. This property is typically populated from the appsettings.json configuration file and is used to configure CORS policies in the application startup.
        /// </summary>
        public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the file path for application logs, allowing for centralized configuration of logging output destinations. This property can be used to specify where log files should be stored, enabling easier management and analysis of application logs for debugging and monitoring purposes. The value is typically set in the appsettings.json configuration file and can be accessed throughout the application to ensure consistent logging behavior.
        /// </summary>
        public string LogPath { get; set; } = string.Empty;
    }
}
