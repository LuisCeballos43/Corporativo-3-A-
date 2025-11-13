using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Corporativo.Models
{
    [Table("REPORTES_MENSUALES")]
    public class ReporteMensual
    {
        [Key]
        [Column("id_reporte")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El ID de región es obligatorio")]
        [Column("id_region")]
        public int IdRegion { get; set; }

        [Required(ErrorMessage = "El año es obligatorio")]
        [Range(2020, 2100, ErrorMessage = "El año debe estar entre 2020 y 2100")]
        [Column("año")]
        public int Año { get; set; }

        [Required(ErrorMessage = "El mes es obligatorio")]
        [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12")]
        [Column("mes")]
        public int Mes { get; set; }

        [Required(ErrorMessage = "El total de ventas es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El total de ventas debe ser mayor o igual a 0")]
        [Column("total_ventas", TypeName = "decimal(12, 2)")]
        public decimal TotalVentas { get; set; }

        [Required(ErrorMessage = "La fecha de recepción es obligatoria")]
        [Column("fecha_recepcion")]
        public DateTime FechaRecepcion { get; set; } = DateTime.Now;

        [ForeignKey("IdRegion")]
        public Region? Region { get; set; }

        public ICollection<DetalleReporte> DetallesReporte { get; set; } = new List<DetalleReporte>();
    }
}
