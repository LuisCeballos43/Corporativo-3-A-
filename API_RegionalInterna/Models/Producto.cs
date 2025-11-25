using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Required]
        public int Existencia { get; set; }

        public bool Activo { get; set; } = true;
    }
}
