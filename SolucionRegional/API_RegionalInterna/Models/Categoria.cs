using System.ComponentModel.DataAnnotations;

namespace API_RegionalInterna.Models
{
    public class Categoria
    {
        [Key] 
        public int Id_Categoria { get; set; }

        [Required]
        public string Categoria_Nombre { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }
}
