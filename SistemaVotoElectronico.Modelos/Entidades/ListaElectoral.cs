using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVotoElectronico.Modelos.Entidades
{
    public class ListaElectoral
    {
        [Key] public int Id { get; set; }
        public string Nombre { get; set; }
        public string NumeroLista { get; set; }
        public string Siglas { get; set; }
        public string Logotipo { get; set; }
        public string Color { get; set; }

        public int? EleccionId { get; set; }

        [ForeignKey("EleccionId")]
        public Eleccion? Eleccion { get; set; }

        public List<Candidato>? Candidatos { get; set; } = new List<Candidato>();
    }
}
