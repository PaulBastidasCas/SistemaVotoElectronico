using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVotoElectronico.Modelos.Entidades
{
    public class Presidente
    {
        [Key] public int Id { get; set; }
        public string Cedula { get; set; }
        public string NombreCompleto { get; set; }
        public string? Fotografia { get; set; }

        //FK
        public int ListaElectoralId { get; set; }
        [ForeignKey("ListaElectoralId")]
        public ListaElectoral? ListaElectoral { get; set; }
    }
}
