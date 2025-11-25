using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_RegionalInterna.Data;
using API_RegionalInterna.Models;
namespace API_RegionalInterna.Controllers
{
    [ApiController]
    [Route("api/corporativo/info-region")]
    public class RegionController : ControllerBase
    {
        private readonly RegionalDbContext _context;

        public RegionController(RegionalDbContext context)
        {
            _context = context;
        }
        // api/corporativo/info-region/id
        [HttpGet("{id}")]
        public IActionResult GetRegionById(int id)
        {
            try
            {
                var region = _context.Regiones
                .Include(r => r.Sucursales)
                    .ThenInclude(s => s.Productos)
                .Include(r => r.Sucursales)
                    .ThenInclude(s => s.ReportesVentas)
                .FirstOrDefault(r => r.Id_Region == id);

                if (region == null)
                {
                    return NotFound(new { status = "error", message = "Región no encontrada" });
                }

                var totalSucursales = region.Sucursales.Count(s => s.Activa);
                var totalProductos = region.Sucursales.Sum(s => s.Productos.Count);

                var reportesRecientes = region.Sucursales
                    .SelectMany(s => s.ReportesVentas)
                    .OrderByDescending(r => r.Año)
                    .ThenByDescending(r => r.Mes)
                    .Take(2)
                    .Select(r => new
                    {
                        año = r.Año,
                        mes = r.Mes,
                        total_ventas = r.Total_Ventas
                    })
                    .ToList();

                var data = new
                {
                    id_region = region.Id_Region,
                    nombre = region.Nombre,
                    total_sucursales = totalSucursales,
                    total_productos = totalProductos,
                    reportes_recientes = reportesRecientes
                };

                return Ok(new { status = "success", data });
            }
            catch (Exception)
            {
                return StatusCode(500, new { status = "error", message = "Error interno del servidor" });
            }
        }
    }
}