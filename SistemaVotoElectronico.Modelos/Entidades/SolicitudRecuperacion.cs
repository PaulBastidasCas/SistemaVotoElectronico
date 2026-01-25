using System.ComponentModel.DataAnnotations;

namespace SistemaVotoElectronico.Modelos.Entidades
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
}