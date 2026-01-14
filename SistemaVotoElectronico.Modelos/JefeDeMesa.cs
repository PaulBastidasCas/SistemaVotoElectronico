using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotoElectronico.Modelos
{
    public class JefeDeMesa
    {
        [Key] public int Id { get; set; }
        public string Cedula { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; } 
        public string Rol { get; set; } = "JefeDeMesa";

        public Mesa? MesaAsignada { get; set; }
    }
}
