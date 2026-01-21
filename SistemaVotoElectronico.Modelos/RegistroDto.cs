using System.ComponentModel.DataAnnotations;

namespace SistemaVotoElectronico.Modelos
{
    public class RegistroDto
    {
        [Required]
        public string Cedula { get; set; }

        [Required]
        public string NombreCompleto { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string Contrasena { get; set; }

        [Required]
        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasena { get; set; }
    }
}
