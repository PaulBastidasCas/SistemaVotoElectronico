using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
