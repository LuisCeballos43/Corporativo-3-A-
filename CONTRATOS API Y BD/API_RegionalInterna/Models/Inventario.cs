using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_RegionalInterna.Models
{
    public class Inventario
    {
        [Key]
        public int Id_Inventario { get; set; }

        [Required]
        public int Id_Sucursal { get; set; }

        [Required]
        public int Id_Producto { get; set; }

        [Required]
        public int Existencia { get; set; }

        [Required]
        public DateTime Ultima_Actualizacion { get; set; }

        // Campo calculado NO guardado en SQL
        [NotMapped]
        public int Sucursales_Disponibles { get; set; }
    }
}
