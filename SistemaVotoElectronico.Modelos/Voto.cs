using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotoElectronico.Modelos
{
    public class Voto
    {
        [Key] public Guid Id { get; set; }

        public int EleccionId { get; set; }

        [ForeignKey("EleccionId")]
        public Eleccion Eleccion { get; set; }

        public int IdCandidatoSeleccionado { get; set; }
        public int IdListaSeleccionada { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
