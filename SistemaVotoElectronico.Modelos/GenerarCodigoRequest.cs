
namespace SistemaVotoElectronico.Modelos
{
    public class GenerarCodigoRequest
    {
        public string CedulaVotante { get; set; }
        public int EleccionId { get; set; }
        public int MesaId { get; set; }
    }
}
