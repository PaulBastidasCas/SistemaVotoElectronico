
namespace SistemaVotoElectronico.Modelos
{
    public class VotoRequest
    {
        public string CodigoEnlace { get; set; } = string.Empty;
        public int EleccionId { get; set; }
        public int? ListaPresidenteId { get; set; }
        public int? ListaAsambleistaId { get; set; }
    }
}
