using System.ComponentModel.DataAnnotations;

namespace API_RegionalInterna.Models
{
    public class Sucursal
    {
        [Key]
        public int Id_Sucursal { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Direccion { get; set; } = string.Empty;

        [Required]
        public string Telefono { get; set; } = string.Empty;

        public bool Activa { get; set; } = true;
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();

        public ICollection<ReportesVentas> ReportesVentas { get; set; } = new List<ReportesVentas>();
        public int Id_Region { get; set; }
        public Region Region { get; set; } = null!;
    }
}
