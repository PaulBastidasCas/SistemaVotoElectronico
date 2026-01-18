using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotoElectronico.Modelos
{
    public class Mesa
    {
        [Key] public int Id { get; set; }
        public string Nombre { get; set; } 
        public string Ubicacion { get; set; }

        public int? EleccionId { get; set; }
        [ForeignKey("EleccionId")]
        public Eleccion? Eleccion { get; set; }

        public int? JefeDeMesaId { get; set; }
        [ForeignKey("JefeDeMesaId")]
        public JefeDeMesa? JefeDeMesa { get; set; }
    }
}
