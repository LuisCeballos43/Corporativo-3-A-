using API_RegionalInterna.Data;
using API_RegionalInterna.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace API_RegionalInterna.Controllers
{
    [ApiController]
    [Route("api/Corporativo/productos")]
    public class ProductosControllerCorporativo : ControllerBase
    {
        private readonly RegionalDbContext _context;

        public ProductosControllerCorporativo(RegionalDbContext context)
        {
            _context = context;
        }
        // GET /api/corporativo/productos
        [HttpGet]
        public IActionResult GetProductosActivos()
        {
            try
            {
                var productos = _context.Productos
                    .Where(p => p.Activo)
                    .Select(p => new
                    {
                        id_producto = p.Id_Producto,
                        nombre = p.Nombre,
                        categoria = p.Categoria,
                        precio = p.Precio,
                        activo = p.Activo
                    })
                    .ToList();

                if (!productos.Any())
                {
                    return NotFound(new { status = "error", message = "No hay productos activos" });
                }

                return Ok(new { status = "success", data = productos });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error interno del servidor"
                });
            }
        }
        //api/corporativo/productos/id
        [HttpGet("{id}")]
        public IActionResult GetProductoById(int id)
        {
            try
            {
                var producto = _context.Productos
                    .Where(p => p.Id_Producto == id)
                    .Select(p => new
                    {
                        id_producto = p.Id_Producto,
                        nombre = p.Nombre,
                        categoria = p.Categoria,
                        precio = p.Precio,
                        activo = p.Activo
                    })
                    .FirstOrDefault();

                if (producto == null)
                {
                    return NotFound(new { status = "error", message = "Producto no encontrado" });
                }

                return Ok(new { status = "success", data = producto });
            }
            catch (Exception)
            {
                return StatusCode(500, new { status = "error", message = "Error interno del servidor" });
            }
        }
    }
}