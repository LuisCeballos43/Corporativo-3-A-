using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_RegionalInterna.Data;
using API_RegionalInterna.Models;

namespace API_RegionalInterna.Controllers
{
    [ApiController]
    [Route("api/interna/reportes-semanales")]
    public class ReportesSemanalesController : ControllerBase
    {
        private readonly RegionalDbContext _context;

        public ReportesSemanalesController(RegionalDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // GET /api/interna/reportes-semanales
        // ==========================================================
        [HttpGet("reportes-semanales")]
        public async Task<IActionResult> GetReportesSemanales()
        {
            var reportes = await _context.ReportesVentas
                .Select(r => new
                {
                    id_reporte = r.Id_Reporte,
                    mes = r.Fecha_Recepcion.Month,
                    año = r.Fecha_Recepcion.Year,
                    ventas_totales = r.Total_Ventas,
                    fecha_recepcion = r.Fecha_Recepcion.ToString("yyyy-MM-dd"),
                    cliente = "Nombre Cliente" // (reemplazar cuando tengas tabla cliente)
                })
                .ToListAsync();

            if (!reportes.Any())
                return NotFound(new { status = "error", message = "No se encontraron reportes" });

            return Ok(new { status = "success", data = reportes });
        }

        // ==========================================================
        // GET /api/interna/reportes-semanales/{id}
        // ==========================================================
        [HttpGet("reportes-semanales/{id}")]
        public async Task<IActionResult> GetReporteById(int id)
        {
            var reporte = await _context.ReportesVentas
                .Where(r => r.Id_Reporte == id)
                .Select(r => new
                {
                    id_reporte = r.Id_Reporte,
                    año = r.Fecha_Recepcion.Year,
                    mes = r.Fecha_Recepcion.Month,
                    ventas_totales = r.Total_Ventas,
                    productos_vendidos = 420, // valor simulado
                    sucursal = "Centro"        // valor simulado
                })
                .FirstOrDefaultAsync();

            if (reporte == null)
                return NotFound(new { status = "error", message = "Reporte no encontrado" });

            return Ok(new { status = "success", data = reporte });
        }

        // ==========================================================
        // GET /api/interna/reportes-semanales/sucursal/{id_sucursal}
        // ==========================================================
        [HttpGet("reportes-semanales/sucursal/{id_sucursal}")]
        public async Task<IActionResult> GetReportesPorSucursal(int id_sucursal)
        {
            var reportes = await _context.ReportesVentas
                .Where(r => r.Id_Sucursal == id_sucursal)
                .Select(r => new
                {
                    mes = r.Fecha_Recepcion.Month,
                    año = r.Fecha_Recepcion.Year,
                    ventas_totales = r.Total_Ventas
                })
                .ToListAsync();

            if (!reportes.Any())
                return NotFound(new { status = "error", message = "Sucursal o reportes no encontrados" });

            return Ok(new { status = "success", data = reportes });
        }

        // ==========================================================
        // GET /api/interna/reportes-mes/{año}/{mes}
        // ==========================================================
        [HttpGet("reportes-mes/{año}/{mes}")]
        public async Task<IActionResult> GetReporteMensual(int año, int mes)
        {
            var reporte = await _context.ReportesVentas
                .Where(r => r.Fecha_Recepcion.Year == año && r.Fecha_Recepcion.Month == mes)
                .GroupBy(r => 1)
                .Select(g => new
                {
                    año = año,
                    mes = mes,
                    ventas_totales = g.Sum(x => x.Total_Ventas),
                    sucursal_top = "Norte",
                    producto_mas_vendido = "Pizza Hawaiana"
                })
                .FirstOrDefaultAsync();

            if (reporte == null)
                return NotFound(new { status = "error", message = "No se encontró el reporte" });

            return Ok(new { status = "success", data = reporte });
        }
    }
}
