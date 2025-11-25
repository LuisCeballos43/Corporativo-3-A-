using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_RegionalInterna.Data;
using API_RegionalInterna.Models;

namespace API_RegionalInterna.Controllers
{
    [ApiController]
    [Route("api/corporativo/reportes-mensuales")]
    public class ReportesMensualesController : ControllerBase
    {
        private readonly RegionalDbContext _context;

        public ReportesMensualesController(RegionalDbContext context)
        {
            _context = context;
        }

        // GET /api/corporativo/reportes-mensuales
        [HttpGet]
        public IActionResult GetReportesMensuales()
        {
            try
            {
                var reportes = _context.ReportesVentas
                    .GroupBy(r => new { r.Año, r.Mes })
                    .Select(g => new
                    {
                        año = g.Key.Año,
                        mes = g.Key.Mes,
                        total_ventas = g.Sum(r => r.Total_Ventas)
                    })
                    .OrderByDescending(r => r.año)
                    .ThenByDescending(r => r.mes)
                    .ToList();

                if (!reportes.Any())
                {
                    return NotFound(new { status = "error", message = "No hay reportes mensuales registrados" });
                }

                return Ok(new { status = "success", data = reportes });
            }
            catch (Exception)
            {
                return StatusCode(500, new { status = "error", message = "Error interno del servidor" });
            }
        }
        // GET /api/corporativo/reportes-mensuales/:año/:mes
        [HttpGet("{año:int}/{mes:int}")]
        public IActionResult GetReporteMensual(int año, int mes)
        {
            try
            {
                var reportesDelMes = _context.ReportesVentas
                    .Where(r => r.Año == año && r.Mes == mes)
                    .ToList();

                if (!reportesDelMes.Any())
                {
                    return NotFound(new { status = "error", message = "No se encontró reporte para ese mes" });
                }

                var totalVentas = reportesDelMes.Sum(r => r.Total_Ventas);
                var sucursalesIncluidas = reportesDelMes.Select(r => r.Id_Sucursal).Distinct().Count();

                var data = new
                {
                    año = año,
                    mes = mes,
                    total_ventas = totalVentas,
                    sucursales_incluidas = sucursalesIncluidas
                };

                return Ok(new { status = "success", data });
            }
            catch (Exception)
            {
                return StatusCode(500, new { status = "error", message = "Error interno del servidor" });
            }
        }

        // GET /api/coporativo/reportes-semanales/sucursal/:id_sucursal
        [HttpGet("sucursal/{id_sucursal:int}")]
        public IActionResult GetReportesSemanalesPorSucursal(int id_sucursal)
        {
            try
            {
                var reportes = _context.ReportesVentas
                    .Where(r => r.Id_Sucursal == id_sucursal)
                    .OrderBy(r => r.Año)
                    .ThenBy(r => r.Mes)
                    .Select(r => new
                    {
                        año = r.Año,
                        mes = r.Mes,
                        total_ventas = r.Total_Ventas
                    })
                    .ToList();

                if (!reportes.Any())
                {
                    return NotFound(new { status = "error", message = "La sucursal no tiene reportes registrados" });
                }

                return Ok(new { status = "success", data = reportes });
            }
            catch (Exception)
            {
                return StatusCode(500, new { status = "error", message = "Error interno del servidor" });
            }
        }
    }
}