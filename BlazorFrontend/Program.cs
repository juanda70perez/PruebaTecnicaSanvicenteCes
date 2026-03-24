using BlazorFrontend.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<BlazorFrontend.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// En producción BaseUrl es "" → usa la URL del propio servidor (API + Blazor en el mismo host)
// En desarrollo BaseUrl apunta a localhost con el puerto del API
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
var baseAddress = string.IsNullOrWhiteSpace(apiBaseUrl)
    ? builder.HostEnvironment.BaseAddress
    : apiBaseUrl;

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(baseAddress),
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PacienteApiService>();

await builder.Build().RunAsync();
