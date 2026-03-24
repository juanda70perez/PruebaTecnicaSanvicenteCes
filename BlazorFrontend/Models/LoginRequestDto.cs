using System.ComponentModel.DataAnnotations;

namespace BlazorFrontend.Models
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [MinLength(3, ErrorMessage = "Mínimo 3 caracteres.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public bool Exito { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public DateTime Expira { get; set; }
    }

    public class CrearPacienteDto
    {
        [Required(ErrorMessage = "El documento es obligatorio.")]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "Entre 5 y 15 caracteres.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Solo se permiten números.")]
        public string Documento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Entre 3 y 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateTime FechaNacimiento { get; set; } = DateTime.Today.AddYears(-20);
    }
}
