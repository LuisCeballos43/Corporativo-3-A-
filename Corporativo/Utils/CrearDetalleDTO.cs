using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Corporativo.Utils
{
    public class CrearDetalleDTO
    {
        [Required(ErrorMessage = "El ID de sucursal es obligatorio")]
        
        public int IdSucursal { get; set; }

        [Required(ErrorMessage = "El ID de producto es obligatorio")]
      
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
       
       
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El subtotal es obligatorio")]

       
        public decimal Subtotal { get; set; }
    }
}
