using Microsoft.AspNetCore.Mvc;
using API_RegionalInterna.Data;
using Microsoft.EntityFrameworkCore;

namespace API_RegionalInterna.Controllers
{
    [ApiController]
    [Route("api/interna/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly RegionalDbContext _context;

        public DashboardController(RegionalDbContext context)
        {
            _context = context;
        }

        // ===============================
        //GET /api/interna/dashboard/resumen
       // ===============================
       [HttpGet("resumen")]
        public IActionResult GetResumen()
        {
            try
            {
                var totalVentas = _context.ReportesVentas.Sum(r => (decimal?)r.Total_Ventas) ?? 0;

                var productosActivos = _context.Productos.Count(p => p.Activo);

                var sucursalesActivas = _context.Sucursales.Count(s => s.Activa);

                var promedioVentas = sucursalesActivas > 0
                    ? totalVentas / sucursalesActivas
                    : 0;

                return Ok(new
                {
                    status = "success",
                    data = new
                    {
                        total_ventas = totalVentas,
                        productos_activos = productosActivos,
                        sucursales_activas = sucursalesActivas,
                        promedio_ventas_por_sucursal = Math.Round(promedioVentas, 2)
                    }
                });
            }
            catch (Exception )
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error interno del servidor",
                });
            }
        }


        // ===============================
        // GET /api/interna/dashboard/ventas-por-sucursal
        // ===============================
        [HttpGet("ventas-por-sucursal")]
        public IActionResult GetVentasPorSucursal()
        {
            try
            {
                var ventas = _context.ReportesVentas
                    .Include(v => v.Sucursal)
                    .GroupBy(v => v.Sucursal.Nombre)
                    .Select(g => new
                    {
                        sucursal = g.Key,
                        ventas = g.Sum(x => x.Total_Ventas)
                    })
                    .ToList();

                return Ok(new
                {
                    status = "success",
                    data = ventas
                });
            }
            catch (Exception )
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error interno del servidor",
                  
                });
            }
        }


        // ===============================
        // GET /api/interna/dashboard/productos-mas-vendidos
        // ===============================
        [HttpGet("productos-mas-vendidos")]
        public IActionResult GetProductosMasVendidos()
        {
            try
            {
                var productos = _context.ReportesVentas
                    .Include(p => p.Producto)
                    .GroupBy(p => p.Producto.Nombre)
                    .Select(g => new
                    {
                        nombre = g.Key,
                        ventas = g.Sum(x => x.Cantidad)
                    })
                    .OrderByDescending(x => x.ventas)
                    .Take(10)
                    .ToList();

                if (productos.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "No hay datos disponibles"
                    });
                }

                return Ok(new
                {
                    status = "success",
                    data = productos
                });
            }
            catch (Exception )
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error interno del servidor",
                
                });
            }
        }
    }
}
