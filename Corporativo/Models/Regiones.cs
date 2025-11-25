
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Corporativo.Models
{
    [Table("REGIONES")]
    public class Region
    {
        [Key]
        [Column("id_region")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la región es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La URL de la API es obligatoria")]
        [StringLength(255, ErrorMessage = "La URL no puede exceder 255 caracteres")]
        [Url(ErrorMessage = "La URL no tiene un formato válido")]
        [Column("api_url")]
        public string ApiUrl { get; set; } = string.Empty;

        [Column("activa")]
        public bool Activa { get; set; } = true;

        public ICollection<ReporteMensual> ReportesMensuales { get; set; } = new List<ReporteMensual>();
    }
}
