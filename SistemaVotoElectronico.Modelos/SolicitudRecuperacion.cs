using System.ComponentModel.DataAnnotations;

namespace SistemaVotoElectronico.Modelos
{
    public class SolicitudRecuperacion
    {
        [Key]
        public int Id { get; set; }
        public string Correo { get; set; }
        public string Token { get; set; } 
        public DateTime Expiracion { get; set; }
        public bool Usado { get; set; } = false;
    }

    public class RecuperarDto
    {
        [Required]
        [EmailAddress]
        public string Correo { get; set; }
    }

    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string NuevaContrasena { get; set; }
    }
}