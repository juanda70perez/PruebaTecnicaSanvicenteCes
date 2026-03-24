using BlazorFrontend.Models;
using System.Net.Http.Json;

namespace BlazorFrontend.Services
{
    public class AuthService
    {
        private readonly HttpClient httpClient;
        private string? token;
        private string? rol;

        public bool EstaAutenticado => !string.IsNullOrEmpty(token);
        public string? Rol => rol;
        public string? Token => token;

        public AuthService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<(bool exito, string mensaje)> LoginAsync(LoginRequestDto loginDto)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/auth/token", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result != null && result.Exito)
                    {
                        token = result.Token;
                        rol = result.Rol;
                        httpClient.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        return (true, $"Bienvenido — Rol: {rol}");
                    }
                }

                return (false, "Usuario o contraseña incorrectos.");
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        public void Logout()
        {
            token = null;
            rol = null;
            httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
