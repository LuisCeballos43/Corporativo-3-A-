using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_RegionalInterna.Models
{
    public class Categoria
    {
        [Key]
        public int Id_Categoria { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public bool Activa { get; set; } = true;
    }
}
