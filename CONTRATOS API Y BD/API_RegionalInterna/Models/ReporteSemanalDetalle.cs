using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_RegionalInterna.Models
{
    public class ReporteSemanalDetalle
    {
        [Key]
        public int Id_Detalle { get; set; }

        [Required]
        public int Id_Reporte { get; set; }

        [ForeignKey("Id_Reporte")]
        public ReportesVentas Reporte { get; set; } = null!;

        [Required]
        public int Id_Producto { get; set; }

        [ForeignKey("Id_Producto")]
        public Producto Producto { get; set; } = null!;

        [Required]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio_Unitario { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio_Venta { get; set; }
    }
}
