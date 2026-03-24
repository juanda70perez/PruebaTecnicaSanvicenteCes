using Api.Extensions;
using Application.Commom;
using Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettingsOptions>(
    builder.Configuration.GetSection(AppSettingsOptions.SectionName));

builder.Services.ConfigureCors(builder.Configuration);
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureDependencyInjection();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.ConfigureSwagger();

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GestionPacientesContext>();
    context.Database.EnsureCreated();
    await DataSeeder.SeedAsync(context);
}

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gestión de Pacientes v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
