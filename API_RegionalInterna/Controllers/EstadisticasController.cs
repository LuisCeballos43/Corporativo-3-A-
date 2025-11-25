using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_RegionalInterna.Data;
using API_RegionalInterna.Models;

[ApiController]
[Route("api/corporativo/estadisticas")]
public class EstadisticasController : ControllerBase
{
    private readonly RegionalDbContext _context;

    public EstadisticasController(RegionalDbContext context)
    {
        _context = context;
    }

    // GET /api/corporativo/estadisticas/mes/:año/:mes
    [HttpGet("mes/{año:int}/{mes:int}")]
    public IActionResult GetEstadisticasMensuales(int año, int mes)
    {
        try
        {
            // Traer todos los reportes del mes solicitado
            var reportesDelMes = _context.ReportesVentas
                .Where(r => r.Año == año && r.Mes == mes)
                .ToList();

            if (!reportesDelMes.Any())
            {
                return NotFound(new { status = "error", message = "No hay estadísticas para ese mes" });
            }

            // Total de ventas
            var ventasTotales = reportesDelMes.Sum(r => r.Total_Ventas);

            // Número de sucursales incluidas
            var sucursalesIncluidas = reportesDelMes.Select(r => r.Id_Sucursal).Distinct().Count();

            // Promedio por sucursal
            var promedioPorSucursal = sucursalesIncluidas > 0
                ? ventasTotales / sucursalesIncluidas
                : 0;

            // Total de productos vendidos (sumando la cantidad de productos por sucursal si existe relación)
            // Aquí asumimos que existe una propiedad CantidadProductosVendidos en ReportesVentas
            var productosVendidos = reportesDelMes.Sum(r => r.ProductosVendidos); // si no existe, habría que adaptarlo

            var data = new
            {
                año = año,
                mes = mes,
                ventas_totales = ventasTotales,
                promedio_por_sucursal = Math.Round(promedioPorSucursal, 2),
                productos_vendidos = productosVendidos
            };

            return Ok(new { status = "success", data });
        }
        catch (Exception)
        {
            return StatusCode(500, new { status = "error", message = "Error interno del servidor" });
        }
    }

    // GET /api/corporativo/estadisticas/top-sucursales/:año/:mes
    [HttpGet("top-sucursales/{año:int}/{mes:int}")]
    public IActionResult GetTopSucursales(int año, int mes)
    {
        try
        {
            // Agrupar por sucursal y sumar las ventas del mes
            var topSucursales = _context.ReportesVentas
                .Include(r => r.Sucursal) // Cargar datos de la sucursal
                .Where(r => r.Año == año && r.Mes == mes)
                .GroupBy(r => new { r.Id_Sucursal, r.Sucursal.Nombre })
                .Select(g => new
                {
                    sucursal = g.Key.Nombre,
                    total_ventas = g.Sum(r => r.Total_Ventas)
                })
                .OrderByDescending(r => r.total_ventas)
                .ToList();

            if (!topSucursales.Any())
            {
                return NotFound(new { status = "error", message = "No se encontraron registros de ventas" });
            }

            return Ok(new { status = "success", data = topSucursales });
        }
        catch (Exception)
        {
            return StatusCode(500, new { status = "error", message = "Error interno del servidor" });
        }
    }
}
