using System.ComponentModel.DataAnnotations;

namespace API_RegionalInterna.Models
{
    public class Producto
    {
        [Key]  
        public int Id_Producto { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Categoria { get; set; } = string.Empty;

        [Required]
        public decimal Precio { get; set; }

        public int Existencia { get; set; }

        public bool Activo { get; set; } = true;
    }
}
