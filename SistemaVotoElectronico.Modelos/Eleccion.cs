using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotoElectronico.Modelos
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
        public List<ListaElectoral>? Listas { get; set; }
        public List<Mesa>? Mesas { get; set; }
    }
}
