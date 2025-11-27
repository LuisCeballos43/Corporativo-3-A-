using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegionalDataBases.Data;
using RegionalDataBases.Models;
using RegionalDataBases.DTOs.Ventas;

namespace RegionalDataBases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== GET: Obtener todas las ventas ==========
        // URL: GET api/Ventas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaDto>>> GetVentas()
        {
            var ventas = await _context.ReportesVenta
                .Include(r => r.Sucursal)
                .Include(r => r.Cliente)
                .Include(r => r.DetallesVenta)
                .Select(r => new VentaDto
                {
                    IdReporte = r.IdReporte,
                    IdSucursal = r.IdSucursal,
                    NombreSucursal = r.Sucursal.Nombre,
                    IdCliente = r.IdCliente,
                    NombreCliente = r.Cliente.Nombre,
                    TotalVentas = r.TotalVentas,
                    FechaRecepcion = r.FechaRecepcion,
                    CantidadProductos = r.DetallesVenta.Count
                })
                .OrderByDescending(v => v.FechaRecepcion)  // Más recientes primero
                .ToListAsync();

            return Ok(ventas);
        }

        // ========== GET: Obtener una venta con todos sus detalles ==========
        // URL: GET api/Ventas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VentaDetalleDto>> GetVenta(int id)
        {
            var venta = await _context.ReportesVenta
                .Include(r => r.Sucursal)
                .Include(r => r.Cliente)
                .Include(r => r.DetallesVenta)
                    .ThenInclude(d => d.Producto)
                .Where(r => r.IdReporte == id)
                .Select(r => new VentaDetalleDto
                {
                    IdReporte = r.IdReporte,
                    TotalVentas = r.TotalVentas,
                    FechaRecepcion = r.FechaRecepcion,
                    Sucursal = new SucursalVentaDto
                    {
                        IdSucursal = r.Sucursal.IdSucursal,
                        Nombre = r.Sucursal.Nombre,
                        Direccion = r.Sucursal.Direccion,
                        Telefono = r.Sucursal.Telefono
                    },
                    Cliente = new ClienteVentaDto
                    {
                        IdCliente = r.Cliente.IdCliente,
                        Nombre = r.Cliente.Nombre,
                        Email = r.Cliente.Email,
                        Telefono = r.Cliente.Telefono
                    },
                    Detalles = r.DetallesVenta.Select(d => new DetalleVentaDto
                    {
                        IdDetalle = d.IdDetalle,
                        IdProducto = d.IdProducto,
                        NombreProducto = d.Producto.Nombre,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        PrecioVenta = d.PrecioVenta
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (venta == null)
            {
                return NotFound(new { message = "Venta no encontrada" });
            }

            return Ok(venta);
        }

        // ========== GET: Ventas por sucursal ==========
        // URL: GET api/Ventas/sucursal/1
        [HttpGet("sucursal/{idSucursal}")]
        public async Task<ActionResult<IEnumerable<VentaDto>>> GetVentasPorSucursal(int idSucursal)
        {
            var sucursalExists = await _context.Sucursales.AnyAsync(s => s.IdSucursal == idSucursal);

            if (!sucursalExists)
            {
                return NotFound(new { message = "Sucursal no encontrada" });
            }

            var ventas = await _context.ReportesVenta
                .Include(r => r.Sucursal)
                .Include(r => r.Cliente)
                .Include(r => r.DetallesVenta)
                .Where(r => r.IdSucursal == idSucursal)
                .Select(r => new VentaDto
                {
                    IdReporte = r.IdReporte,
                    IdSucursal = r.IdSucursal,
                    NombreSucursal = r.Sucursal.Nombre,
                    IdCliente = r.IdCliente,
                    NombreCliente = r.Cliente.Nombre,
                    TotalVentas = r.TotalVentas,
                    FechaRecepcion = r.FechaRecepcion,
                    CantidadProductos = r.DetallesVenta.Count
                })
                .OrderByDescending(v => v.FechaRecepcion)
                .ToListAsync();

            return Ok(ventas);
        }

        // ========== GET: Ventas por cliente ==========
        // URL: GET api/Ventas/cliente/1
        [HttpGet("cliente/{idCliente}")]
        public async Task<ActionResult<IEnumerable<VentaDto>>> GetVentasPorCliente(int idCliente)
        {
            var clienteExists = await _context.Clientes.AnyAsync(c => c.IdCliente == idCliente);

            if (!clienteExists)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }

            var ventas = await _context.ReportesVenta
                .Include(r => r.Sucursal)
                .Include(r => r.Cliente)
                .Include(r => r.DetallesVenta)
                .Where(r => r.IdCliente == idCliente)
                .Select(r => new VentaDto
                {
                    IdReporte = r.IdReporte,
                    IdSucursal = r.IdSucursal,
                    NombreSucursal = r.Sucursal.Nombre,
                    IdCliente = r.IdCliente,
                    NombreCliente = r.Cliente.Nombre,
                    TotalVentas = r.TotalVentas,
                    FechaRecepcion = r.FechaRecepcion,
                    CantidadProductos = r.DetallesVenta.Count
                })
                .OrderByDescending(v => v.FechaRecepcion)
                .ToListAsync();

            return Ok(ventas);
        }

        // ========== GET: Ventas por rango de fechas ==========
        // URL: GET api/Ventas/por-fecha?inicio=2025-01-01&fin=2025-12-31
        [HttpGet("por-fecha")]
        public async Task<ActionResult<IEnumerable<VentaDto>>> GetVentasPorFecha(
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fin)
        {
            if (inicio > fin)
            {
                return BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha fin" });
            }

            var ventas = await _context.ReportesVenta
                .Include(r => r.Sucursal)
                .Include(r => r.Cliente)
                .Include(r => r.DetallesVenta)
                .Where(r => r.FechaRecepcion >= inicio && r.FechaRecepcion <= fin)
                .Select(r => new VentaDto
                {
                    IdReporte = r.IdReporte,
                    IdSucursal = r.IdSucursal,
                    NombreSucursal = r.Sucursal.Nombre,
                    IdCliente = r.IdCliente,
                    NombreCliente = r.Cliente.Nombre,
                    TotalVentas = r.TotalVentas,
                    FechaRecepcion = r.FechaRecepcion,
                    CantidadProductos = r.DetallesVenta.Count
                })
                .OrderByDescending(v => v.FechaRecepcion)
                .ToListAsync();

            return Ok(ventas);
        }

        // ========== POST: Crear una venta completa ==========
        // URL: POST api/Ventas
        // Body: { "idSucursal": 1, "idCliente": 1, "detalles": [{"idProducto": 1, "cantidad": 2}, {"idProducto": 3, "cantidad": 1}] }
        [HttpPost]
        public async Task<ActionResult<VentaDetalleDto>> CreateVenta(VentaCreateDto ventaDto)
        {
            // 1. Validar que la sucursal existe
            var sucursalExists = await _context.Sucursales
                .AnyAsync(s => s.IdSucursal == ventaDto.IdSucursal);

            if (!sucursalExists)
            {
                return BadRequest(new { message = "La sucursal especificada no existe" });
            }

            // 2. Validar que el cliente existe
            var clienteExists = await _context.Clientes
                .AnyAsync(c => c.IdCliente == ventaDto.IdCliente);

            if (!clienteExists)
            {
                return BadRequest(new { message = "El cliente especificado no existe" });
            }

            // 3. Validar que hay detalles
            if (ventaDto.Detalles == null || !ventaDto.Detalles.Any())
            {
                return BadRequest(new { message = "Debe incluir al menos un producto en la venta" });
            }

            // 4. Validar y obtener información de cada producto
            decimal totalVenta = 0;
            var detallesVenta = new List<DetalleVenta>();

            foreach (var detalle in ventaDto.Detalles)
            {
                // Validar cantidad
                if (detalle.Cantidad <= 0)
                {
                    return BadRequest(new { message = $"La cantidad del producto {detalle.IdProducto} debe ser mayor a 0" });
                }

                // Obtener el producto
                var producto = await _context.Productos.FindAsync(detalle.IdProducto);

                if (producto == null)
                {
                    return BadRequest(new { message = $"El producto con ID {detalle.IdProducto} no existe" });
                }

                if (!producto.Activo)
                {
                    return BadRequest(new { message = $"El producto '{producto.Nombre}' no está activo" });
                }

                // Verificar inventario en la sucursal
                var inventario = await _context.Inventarios
                    .FirstOrDefaultAsync(i => i.IdSucursal == ventaDto.IdSucursal
                                           && i.IdProducto == detalle.IdProducto);

                if (inventario == null)
                {
                    return BadRequest(new { message = $"El producto '{producto.Nombre}' no está disponible en esta sucursal" });
                }

                if (inventario.Existencia < detalle.Cantidad)
                {
                    return BadRequest(new { message = $"No hay suficiente inventario de '{producto.Nombre}'. Disponible: {inventario.Existencia}" });
                }

                // Calcular subtotal
                decimal precioVenta = producto.Precio * detalle.Cantidad;
                totalVenta += precioVenta;

                // Crear detalle de venta
                detallesVenta.Add(new DetalleVenta
                {
                    IdProducto = detalle.IdProducto,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = producto.Precio,
                    PrecioVenta = precioVenta
                });

                // Actualizar inventario (restar la cantidad vendida)
                inventario.Existencia -= detalle.Cantidad;
                inventario.UltimaActualizacion = DateTime.Now;
            }

            // 5. Crear el reporte de venta
            var reporteVenta = new ReporteVenta
            {
                IdSucursal = ventaDto.IdSucursal,
                IdCliente = ventaDto.IdCliente,
                TotalVentas = totalVenta,
                FechaRecepcion = DateTime.Now,
                DetallesVenta = detallesVenta
            };

            _context.ReportesVenta.Add(reporteVenta);
            await _context.SaveChangesAsync();

            // 6. Obtener la venta completa para devolver
            var ventaResponse = await _context.ReportesVenta
                .Include(r => r.Sucursal)
                .Include(r => r.Cliente)
                .Include(r => r.DetallesVenta)
                    .ThenInclude(d => d.Producto)
                .Where(r => r.IdReporte == reporteVenta.IdReporte)
                .Select(r => new VentaDetalleDto
                {
                    IdReporte = r.IdReporte,
                    TotalVentas = r.TotalVentas,
                    FechaRecepcion = r.FechaRecepcion,
                    Sucursal = new SucursalVentaDto
                    {
                        IdSucursal = r.Sucursal.IdSucursal,
                        Nombre = r.Sucursal.Nombre,
                        Direccion = r.Sucursal.Direccion,
                        Telefono = r.Sucursal.Telefono
                    },
                    Cliente = new ClienteVentaDto
                    {
                        IdCliente = r.Cliente.IdCliente,
                        Nombre = r.Cliente.Nombre,
                        Email = r.Cliente.Email,
                        Telefono = r.Cliente.Telefono
                    },
                    Detalles = r.DetallesVenta.Select(d => new DetalleVentaDto
                    {
                        IdDetalle = d.IdDetalle,
                        IdProducto = d.IdProducto,
                        NombreProducto = d.Producto.Nombre,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        PrecioVenta = d.PrecioVenta
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetVenta), new { id = reporteVenta.IdReporte }, ventaResponse);
        }

        // ========== DELETE: Eliminar una venta ==========
        // URL: DELETE api/Ventas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenta(int id)
        {
            var venta = await _context.ReportesVenta
                .Include(r => r.DetallesVenta)
                .FirstOrDefaultAsync(r => r.IdReporte == id);

            if (venta == null)
            {
                return NotFound(new { message = "Venta no encontrada" });
            }

            // Eliminar primero los detalles (por integridad referencial)
            _context.DetallesVenta.RemoveRange(venta.DetallesVenta);

            // Eliminar el reporte
            _context.ReportesVenta.Remove(venta);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ========== REPORTES Y ESTADÍSTICAS ==========

        // GET: Total de ventas por sucursal
        // URL: GET api/Ventas/reporte/por-sucursal
        [HttpGet("reporte/por-sucursal")]
        public async Task<ActionResult<IEnumerable<VentasPorSucursalDto>>> GetVentasPorSucursalReporte()
        {
            var reporte = await _context.ReportesVenta
                .Include(r => r.Sucursal)
                .GroupBy(r => new { r.IdSucursal, r.Sucursal.Nombre })
                .Select(g => new VentasPorSucursalDto
                {
                    IdSucursal = g.Key.IdSucursal,
                    NombreSucursal = g.Key.Nombre,
                    TotalVentas = g.Sum(r => r.TotalVentas),
                    CantidadVentas = g.Count()
                })
                .OrderByDescending(r => r.TotalVentas)
                .ToListAsync();

            return Ok(reporte);
        }

        // GET: Productos más vendidos
        // URL: GET api/Ventas/reporte/productos-mas-vendidos?top=10
        [HttpGet("reporte/productos-mas-vendidos")]
        public async Task<ActionResult<IEnumerable<ProductoMasVendidoDto>>> GetProductosMasVendidos([FromQuery] int top = 10)
        {
            var productos = await _context.DetallesVenta
                .Include(d => d.Producto)
                .GroupBy(d => new { d.IdProducto, d.Producto.Nombre })
                .Select(g => new ProductoMasVendidoDto
                {
                    IdProducto = g.Key.IdProducto,
                    NombreProducto = g.Key.Nombre,
                    CantidadVendida = g.Sum(d => d.Cantidad),
                    TotalGenerado = g.Sum(d => d.PrecioVenta)
                })
                .OrderByDescending(p => p.CantidadVendida)
                .Take(top)
                .ToListAsync();

            return Ok(productos);
        }

        // GET: Ventas por día
        // URL: GET api/Ventas/reporte/por-dia?dias=30
        [HttpGet("reporte/por-dia")]
        public async Task<ActionResult<IEnumerable<VentasPorFechaDto>>> GetVentasPorDia([FromQuery] int dias = 30)
        {
            var fechaInicio = DateTime.Now.AddDays(-dias);

            var reporte = await _context.ReportesVenta
                .Where(r => r.FechaRecepcion >= fechaInicio)
                .GroupBy(r => r.FechaRecepcion.Date)
                .Select(g => new VentasPorFechaDto
                {
                    Fecha = g.Key,
                    TotalVentas = g.Sum(r => r.TotalVentas),
                    CantidadVentas = g.Count()
                })
                .OrderBy(r => r.Fecha)
                .ToListAsync();

            return Ok(reporte);
        }
    }
}