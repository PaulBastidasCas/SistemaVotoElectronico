using SistemaVotoElectronico.Modelos.Entidades;
using System.ComponentModel.DataAnnotations;

namespace SistemaVotoElectronico.Modelos
{
    public class Votante
    {
        [Key] public int Id { get; set; }
        public string Cedula { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string? Contrasena { get; set; }
        public string? Fotografia { get; set; }

        public string Rol { get; set; } = "Votante";

        public List<PadronElectoral>? HistorialVotos { get; set; } = new List<PadronElectoral>();
    }
}
