using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_RegionalInterna.Data;
using API_RegionalInterna.Models;
using System.Text.Json.Serialization;

namespace API_RegionalInterna.Controllers
{
    [ApiController]
    [Route("api/interna/inventario")]
    public class InventarioController : ControllerBase
    {
        private readonly RegionalDbContext _context;

        public InventarioController(RegionalDbContext context)
        {
            _context = context;
        }

        // DTO para actualizar existencia
        public class ActualizarExistenciaDto
        {
            [JsonPropertyName("nueva_existencia")]
            public int NuevaExistencia { get; set; }
        }

        // ============================
        // GET /api/interna/inventario
        // Inventario total agrupado por producto
        // ============================
        [HttpGet]
        public IActionResult GetInventarioTotal()
        {
            try
            {
                var inventario = _context.Inventarios
                    .GroupBy(i => i.Id_Producto)
                    .Select(g => new
                    {
                        id_producto = g.Key,
                        nombre = _context.Productos
                                    .Where(p => p.Id_Producto == g.Key)
                                    .Select(p => p.Nombre)
                                    .FirstOrDefault(),
                        existencia_total = g.Sum(x => x.Existencia),
                        sucursales_disponibles = g.Count(x => x.Existencia > 0)
                    })
                    .ToList();

                if (!inventario.Any())
                    return NotFound(new { status = "error", message = "No se encontraron productos" });

                return Ok(new { status = "success", data = inventario });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        // ============================
        // GET /api/inventario/sucursal/{id}
        // Inventario de una sucursal
        // ============================
        [HttpGet("sucursal/{id}")]
        public IActionResult GetInventarioPorSucursal(int id)
        {
            try
            {
                bool sucursalExiste = _context.Sucursales.Any(s => s.Id_Sucursal == id);
                if (!sucursalExiste)
                    return NotFound(new { status = "error", message = "Sucursal no encontrada" });

                var inventario = _context.Inventarios
                    .Where(i => i.Id_Sucursal == id)
                    .Select(i => new
                    {
                        id_producto = i.Id_Producto,
                        nombre = _context.Productos
                                    .Where(p => p.Id_Producto == i.Id_Producto)
                                    .Select(p => p.Nombre)
                                    .FirstOrDefault(),
                        existencia = i.Existencia
                    })
                    .ToList();

                return Ok(new { status = "success", data = inventario });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        // ============================
        // PATCH /api/inventario/existencia/{id_producto}/{id_sucursal?}
        // Actualizar existencia
        // ============================
        [HttpPatch("existencia/{id_producto}")]
        public IActionResult ActualizarExistencia(int id_producto, ActualizarExistenciaDto body)
        {
            try
            {
                if (body == null)
                    return BadRequest(new { status = "error", message = "Datos inválidos o incompletos" });

                var inventarios = _context.Inventarios
                    .Where(i => i.Id_Producto == id_producto)
                    .ToList();

                if (!inventarios.Any())
                    return NotFound(new { status = "error", message = "Producto no encontrado" });

                foreach (var inv in inventarios)
                {
                    inv.Existencia = body.NuevaExistencia;
                    inv.Ultima_Actualizacion = DateTime.Now;
                }

                _context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Existencia actualizada correctamente",
                    data = new
                    {
                        id_producto,
                        nueva_existencia = body.NuevaExistencia
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = $"Error interno del servidor: {ex.Message}" });
            }
        }

    }
}
