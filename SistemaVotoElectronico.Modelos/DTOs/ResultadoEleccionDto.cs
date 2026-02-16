using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotoElectronico.Modelos.DTOs
{
    public class ResultadoEleccionDto
    {
        public string Eleccion { get; set; }
        public int TotalVotosPresidente { get; set; }
        public int TotalVotosAsambleistas { get; set; }
        public int TotalEscanos { get; set; }
        public DateTime FechaCorte { get; set; }
        public List<DetalleListaDto> ResultadosPresidente { get; set; }
        public List<DetalleListaDto> ResultadosAsambleistas { get; set; }
    }
}
