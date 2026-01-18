using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Models
{
    public class PapeletaViewModel
    {
        public Eleccion Eleccion { get; set; }
        public List<ListaElectoral> Listas { get; set; }
        public string CodigoEnlace { get; set; }
    }
}
