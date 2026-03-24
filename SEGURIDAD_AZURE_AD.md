# Seguridad con Azure AD — Endpoints de Gestión de Pacientes

## Estrategia de Protección

Para proteger los endpoints de esta API (`/api/pacientes`) usando **Azure Active Directory**, se seguiría el flujo **OAuth 2.0 + Bearer Token** con la librería `Microsoft.Identity.Web`.

---

## 1. Registro de la Aplicación en Azure AD

1. Ir al portal de Azure → **Azure Active Directory** → **App registrations** → **New registration**.
2. Configurar:
   - **Nombre:** `GestionPacientesApi`
   - **Redirect URI:** `https://localhost:7001` (o la URL de producción)
3. En **Expose an API**, crear un scope personalizado:
   - `api://<client-id>/Pacientes.ReadWrite`
4. En **App roles** (opcional), definir roles como `PacientesAdmin`, `PacientesViewer` para control granular.

---

## 2. Cambios en el Proyecto

### Instalar el paquete:
```bash
dotnet add package Microsoft.Identity.Web
```

### `appsettings.json` — agregar sección AzureAd:
```json
"AzureAd": {
  "Instance": "https://login.microsoftonline.com/",
  "TenantId": "<your-tenant-id>",
  "ClientId": "<your-client-id>",
  "Audience": "api://<your-client-id>"
}
```

### `Program.cs` — agregar autenticación:
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// ...
app.UseAuthentication();
app.UseAuthorization();
```

---

## 3. Proteger los Endpoints

Agregar `[Authorize]` al controlador o a cada acción según el nivel de acceso requerido:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere token válido de Azure AD
public class PacientesController : ControllerBase
{
    // GET: acceso de solo lectura
    [HttpGet]
    [Authorize(Roles = "PacientesViewer,PacientesAdmin")]
    public async Task<IActionResult> ObtenerTodos(...) { ... }

    // POST: solo administradores pueden registrar
    [HttpPost]
    [Authorize(Roles = "PacientesAdmin")]
    public async Task<IActionResult> Registrar(...) { ... }
}
```

---

## 4. Validaciones Automáticas del Token

Al usar `Microsoft.Identity.Web`, se validan automáticamente:
- **Firma del token** (firmado por Azure AD con RS256)
- **Audiencia (`aud`)**: debe coincidir con el `ClientId` de la app
- **Emisor (`iss`)**: debe ser el tenant de Azure AD configurado
- **Expiración (`exp`)**: tokens expirados son rechazados con `401 Unauthorized`

---

## 5. Flujo Resumido

```
Cliente (Postman / Frontend / App)
        |
        | 1. Solicita token → POST https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token
        |
        | 2. Azure AD valida credenciales y retorna JWT Bearer Token
        |
        | 3. Cliente envía request con header:
        |    Authorization: Bearer <token>
        |
        v
API ASP.NET Core
        |
        | 4. Middleware valida token automáticamente
        |
        | 5. Si válido → ejecuta acción del controlador
        | 6. Si inválido/expirado → retorna 401 Unauthorized
```

---

## 6. Consideraciones Adicionales para Producción

- **HTTPS obligatorio** en todos los entornos (`UseHttpsRedirection`).
- **Secrets en Azure Key Vault**, no en `appsettings.json`.
- Habilitar **Conditional Access** en Azure AD para exigir MFA a usuarios con datos sensibles.
- Registrar en **Azure Monitor / Application Insights** los intentos fallidos de autenticación.
- Aplicar **rate limiting** en los endpoints de escritura para prevenir ataques de fuerza bruta.
