namespace BlazorFrontend.Models
{
    public class PacienteDto
    {
        public string Documento { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
    }
}
