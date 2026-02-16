namespace SistemaVotoElectronico.MVC.Models
{
    public class PerfilViewModel
    {
        public string Nombre { get; set; }
        public string? Foto { get; set; }
        public string Rol { get; set; }
        public string Correo { get; set; }
        public string Stat1Label { get; set; }
        public string Stat1Value { get; set; }
        public string Stat2Label { get; set; }
        public string Stat2Value { get; set; }
        public List<dynamic> Items { get; set; } = new List<dynamic>();
    }
}
