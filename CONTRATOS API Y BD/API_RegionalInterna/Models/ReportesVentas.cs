using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_RegionalInterna.Models
{
    public class ReportesVentas
    {
  

        [Key]
        public int Id_Reporte { get; set; }

        [Required]
        public int Id_Sucursal { get; set; }

        [ForeignKey("Id_Sucursal")]
        public Sucursal Sucursal { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total_Ventas { get; set; }

        public DateTime Fecha_Recepcion { get; set; } = DateTime.Now;

        public int Año { get; set; }
        public int Mes { get; set; }

        [Required]
        public int Id_Cliente { get; set; }

        [Required]
        public int Id_Producto { get; set; }

        [ForeignKey("Id_Producto")]
        public Producto Producto { get; set; } = null!;

        // Nueva propiedad
        public int ProductosVendidos { get; set; } = 0;

        [Required]
        public int Cantidad { get; set; }
        public List<ReporteSemanalDetalle> Detalles { get; set; } = new();
    }


}
