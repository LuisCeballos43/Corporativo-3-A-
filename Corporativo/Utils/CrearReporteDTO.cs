using System.ComponentModel.DataAnnotations;
namespace Corporativo.Utils
{
    public class CrearReporteDTO
    {
        [Required(ErrorMessage = "El ID de región es obligatorio")]
        public int IdRegion { get; set; }

        [Required(ErrorMessage = "El año es obligatorio")]
        [Range(2020, 2100, ErrorMessage = "El año debe estar entre 2020 y 2100")]
        public int Año { get; set; }

        [Required(ErrorMessage = "El mes es obligatorio")]
        [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12")]
        public int Mes { get; set; }

        [Required(ErrorMessage = "Los detalles del reporte son obligatorios")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un detalle")]
        public List<CrearDetalleDTO> Detalles { get; set; } = new List<CrearDetalleDTO>();
    }
}
