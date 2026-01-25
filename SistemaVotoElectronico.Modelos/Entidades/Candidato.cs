using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVotoElectronico.Modelos.Entidades
{
    public class Candidato
    {
        [Key] public int Id { get; set; }
        public string Cedula { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string? Contrasena { get; set; }
        public string? Fotografia { get; set; }

        public string Rol { get; set; } = "Candidato";

        public int OrdenEnLista { get; set; }

        //FK
        public int? ListaElectoralId { get; set; }

        [ForeignKey("ListaElectoralId")]
        public ListaElectoral? ListaElectoral { get; set; }
    }
}
