using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotoElectronico.Modelos
{
    public class PadronElectoral
    {
        [Key] public int Id { get; set; }

        public int VotanteId { get; set; }
        [ForeignKey("VotanteId")]
        public Votante Votante { get; set; }

        public int EleccionId { get; set; }

        [ForeignKey("EleccionId")]
        public Eleccion Eleccion { get; set; }

        public bool VotoPlanchaRealizado { get; set; }
        public bool VotoNominalRealizado { get; set; }
        public DateTime FechaVoto { get; set; }
    }
}
