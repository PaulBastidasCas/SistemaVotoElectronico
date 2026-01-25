using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotoElectronico.Modelos.DTOs
{
    public class DetalleListaDto
    {
        public string Lista { get; set; }
        public string Siglas { get; set; }
        public string Color { get; set; }
        public int VotosTotales { get; set; }
        public double Porcentaje { get; set; }
        public int EscanosAsignados { get; set; }
    }
}
