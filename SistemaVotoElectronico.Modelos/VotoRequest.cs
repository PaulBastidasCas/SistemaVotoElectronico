
namespace SistemaVotoElectronico.Modelos
{
    public class VotoRequest
    {
        public string CodigoEnlace { get; set; }
        public int EleccionId { get; set; }
        public int? IdCandidatoSeleccionado { get; set; }
        public int? IdListaSeleccionada { get; set; }
    }
}
