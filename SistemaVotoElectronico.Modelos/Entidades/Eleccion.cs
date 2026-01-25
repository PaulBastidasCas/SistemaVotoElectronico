using System.ComponentModel.DataAnnotations;

namespace SistemaVotoElectronico.Modelos.Entidades
{
    public class Eleccion
    {
        [Key] public int Id { get; set; }
        public string Nombre { get; set; }
        public string TipoEleccion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinalizacion { get; set; }
        public bool Activa { get; set; }

        //Relaciones
        public List<ListaElectoral>? Listas { get; set; }= new List<ListaElectoral>();
        public List<Mesa>? Mesas { get; set; }= new List<Mesa>();
    }
}
