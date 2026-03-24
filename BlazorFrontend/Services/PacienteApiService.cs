using BlazorFrontend.Models;
using System.Net.Http.Json;

namespace BlazorFrontend.Services
{
    public class PacienteApiService
    {
        private readonly HttpClient httpClient;

        public PacienteApiService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<PacienteDto>> ObtenerTodosAsync(string? filter = null, int take = 10)
        {
            var query = new List<string> { $"take={take}" };
            if (!string.IsNullOrWhiteSpace(filter))
                query.Add($"filter={Uri.EscapeDataString(filter)}");

            var url = $"api/pacientes?{string.Join("&", query)}";

            var result = await httpClient.GetFromJsonAsync<List<PacienteDto>>(url);
            return result ?? new List<PacienteDto>();
        }

        public async Task<(bool exito, string mensaje)> CrearAsync(CrearPacienteDto dto)
        {
            try
            {
                var payload = new
                {
                    documento = dto.Documento,
                    nombre = dto.Nombre,
                    email = dto.Email,
                    fechaNacimiento = dto.FechaNacimiento,
                };

                var response = await httpClient.PostAsJsonAsync("api/pacientes", payload);
                var content = await response.Content.ReadFromJsonAsync<ApiResponse>();

                if (response.IsSuccessStatusCode)
                    return (true, content?.Mensaje ?? "Paciente registrado.");

                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    return (false, content?.Mensaje ?? "Ya existe un paciente activo con ese documento.");

                return (false, content?.Mensaje ?? "Error al registrar.");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool exito, string mensaje)> ActualizarAsync(int id, ActualizarPacienteDto dto)
        {
            try
            {
                var response = await httpClient.PutAsJsonAsync($"api/pacientes/{id}", dto);
                var content = await response.Content.ReadFromJsonAsync<ApiResponse>();

                if (response.IsSuccessStatusCode)
                    return (true, content?.Mensaje ?? "Paciente actualizado.");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return (false, content?.Mensaje ?? "Paciente no encontrado.");

                return (false, content?.Mensaje ?? "Error al actualizar.");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
    }

    public class ApiResponse
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
