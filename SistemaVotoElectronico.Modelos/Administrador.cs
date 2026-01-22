using System.ComponentModel.DataAnnotations;

namespace SistemaVotoElectronico.Modelos
{
    public class Administrador
    {
        [Key] public int Id { get; set; }
        public string Cedula { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string? Contrasena { get; set; }
        public string? Fotografia { get; set; }
        public string Rol { get; set; } = "Administrador";
    }
}
