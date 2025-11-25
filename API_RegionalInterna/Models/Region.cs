using System.ComponentModel.DataAnnotations;

namespace API_RegionalInterna.Models
{
    public class Region
    {
        [Key]
        public int Id_Region { get; set; }

        [Required]
        public string Nombre { get; set; } = null!;

        public ICollection<Sucursal> Sucursales { get; set; } = new List<Sucursal>();
    }

}
