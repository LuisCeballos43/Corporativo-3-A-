using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corporativo.Models
{
    [Table("DETALLE_REPORTE")]
    public class DetalleReporte
    {
        [Key]
        [Column("id_detalle")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El ID del reporte es obligatorio")]
        [Column("id_reporte")]
        public int IdReporte { get; set; }

        [Required(ErrorMessage = "El ID de sucursal es obligatorio")]
        [Column("id_sucursal")]
        public int IdSucursal { get; set; }

        [Required(ErrorMessage = "El ID de producto es obligatorio")]
        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El subtotal es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El subtotal debe ser mayor o igual a 0")]
        [Column("subtotal", TypeName = "decimal(10, 2)")]
        public decimal Subtotal { get; set; }

        [ForeignKey("IdReporte")]
        public ReporteMensual? ReporteMensual { get; set; }
    }
}
