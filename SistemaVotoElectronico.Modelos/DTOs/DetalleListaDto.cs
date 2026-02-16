using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotoElectronico.Modelos.DTOs
{
    public class DetalleListaDto
    {
        public string Lista { get; set; } = string.Empty;
        public string Siglas { get; set; } = string.Empty;
        public string Color { get; set; } = "#000000";
        public int VotosTotales { get; set; }
        public double Porcentaje { get; set; }
        public int EscanosAsignados { get; set; }
    }
}
