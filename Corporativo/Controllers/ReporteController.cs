using Corporativo.Data;
using Corporativo.Models;
using Corporativo.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Corporativo.Controllers
{
    [Route("/corporativo/api/reportes-mensuales")]
    public class ReporteController : Controller
    {
        private readonly CorporativoContext _context;
        public ReporteController(CorporativoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReporteMensual>>> GetReportesMensuales()
        {
            var reportes = await _context.ReportesMensuales
                .Include(r => r.Region)
                .Include(r => r.DetallesReporte)
                .ToListAsync();

            return Ok(reportes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReporteMensual>> GetReporteMensual(int id)
        {
            var reporte = await _context.ReportesMensuales
                .Include(r => r.Region)
                .Include(r => r.DetallesReporte)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reporte == null)
            {
                return NotFound(new { mensaje = "Reporte no encontrado" });
            }

            return Ok(reporte);
        }

       
        [HttpPost]
        public async Task<ActionResult<ReporteMensual>> PostReporteMensual([FromBody] CrearReporteDTO reporteDTO)
        {
            
            if (reporteDTO == null)
            {
                return BadRequest(new { mensaje = "No se recibieron datos" });
            }

          
            // Validar que haya al menos un detalle
            if (reporteDTO.Detalles.Count() <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El reporte debe incluir al menos un detalle",
                    cantidadDetalles = reporteDTO.Detalles?.Count ?? 0,
                    detalles = reporteDTO == null,

                });
            }

            //validar que no haya otro reporte en el mismo mes
            var otroreporte = await _context.ReportesMensuales.AnyAsync(r => r.Mes == reporteDTO.Mes && r.Año == reporteDTO.Año);
            if (otroreporte)
            {
                return BadRequest(new
                {
                   mensaje = "Ya existe un reporte para ese mes y año"
                });
            }

          
            foreach (var det in reporteDTO.Detalles)
            {
                Console.WriteLine($"  - Sucursal: {det.IdSucursal}, Producto: {det.IdProducto}, Cantidad: {det.Cantidad}, Subtotal: {det.Subtotal}");
            }

            // Validar que la región exista
            try
            {
                var region = await _context.Regiones.FindAsync(reporteDTO.IdRegion);

                if (region == null)
                {
                    var todasLasRegiones = await _context.Regiones.ToListAsync();
                    return BadRequest(new
                    {
                        mensaje = "La región especificada no existe",
                        idBuscado = reporteDTO.IdRegion,
                        regionesDisponibles = todasLasRegiones.Select(r => new { r.Id, r.Nombre }).ToList(),
                        totalRegiones = todasLasRegiones.Count
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al validar la región",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }

            // Calcular el total de ventas sumando los subtotales de los detalles
            decimal totalVentas = reporteDTO.Detalles.Sum(d => d.Subtotal);

            // Crear el reporte mensual
            var reporte = new ReporteMensual
            {
                IdRegion = reporteDTO.IdRegion,
                Año = reporteDTO.Año,
                Mes = reporteDTO.Mes,
                TotalVentas = totalVentas,
                FechaRecepcion = DateTime.Now
            };

            // Crear los detalles del reporte
            foreach (var detalleDTO in reporteDTO.Detalles)
            {
                var detalle = new DetalleReporte
                {
                    IdSucursal = detalleDTO.IdSucursal,
                    IdProducto = detalleDTO.IdProducto,
                    Cantidad = detalleDTO.Cantidad,
                    Subtotal = detalleDTO.Subtotal
                };

                reporte.DetallesReporte.Add(detalle);
            }

            // Iniciar una transacción para asegurar que todo se guarde correctamente
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.ReportesMensuales.Add(reporte);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Cargar las relaciones para devolver el objeto completo
                await _context.Entry(reporte)
                    .Reference(r => r.Region)
                    .LoadAsync();

                return CreatedAtAction(
                    nameof(GetReporteMensual),
                    new { id = reporte.Id },
                    new
                    {
                        mensaje = "Reporte creado exitosamente",
                        reporte = reporte,
                        detallesCreados = reporte.DetallesReporte.Count
                    });
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();

                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                var innerInnerMessage = dbEx.InnerException?.InnerException?.Message;

                return StatusCode(500, new
                {
                    mensaje = "Error al guardar en la base de datos",
                    error = dbEx.Message,
                    detalleError = innerMessage,
                    detalleAdicional = innerInnerMessage,
                    stackTrace = dbEx.StackTrace
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return StatusCode(500, new
                {
                    mensaje = "Error inesperado al crear el reporte",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message,
                    tipo = ex.GetType().Name
                });
            }
        }

        [HttpGet("region/{idRegion}")]
        public async Task<ActionResult<IEnumerable<ReporteMensual>>> GetReportesPorRegion(int idRegion)
        {
            var reportes = await _context.ReportesMensuales
                .Include(r => r.Region)
                .Include(r => r.DetallesReporte)
                .Where(r => r.IdRegion == idRegion)
                .OrderByDescending(r => r.Año)
                .ThenByDescending(r => r.Mes)
                .ToListAsync();

            if (!reportes.Any())
            {
                return NotFound(new { mensaje = "No se encontraron reportes para esta región" });
            }

            return reportes;
        }

        [HttpGet("periodo/{año}/{mes}")]
        public async Task<ActionResult<IEnumerable<ReporteMensual>>> GetReportesPorPeriodo(int año, int mes)
        {
            if (mes < 1 || mes > 12)
            {
                return BadRequest(new { mensaje = "El mes debe estar entre 1 y 12" });
            }

            var reportes = await _context.ReportesMensuales
                .Include(r => r.Region)
                .Include(r => r.DetallesReporte)
                .Where(r => r.Año == año && r.Mes == mes)
                .ToListAsync();

            if (!reportes.Any())
            {
                return NotFound(new { mensaje = $"No se encontraron reportes para {mes}/{año}" });
            }

            return reportes;
        }

    }
}
