using API_RegionalInterna.Data;
using API_RegionalInterna.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace API_RegionalInterna.Controllers
{
    [ApiController]
    [Route("api/sucursal/productos")]
    public class ProductosControllerSucursal : ControllerBase
    {
        private readonly RegionalDbContext _context;

        public ProductosControllerSucursal(RegionalDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1) CATÁLOGO DE PRODUCTOS POR SUCURSAL  (SOLO LECTURA)
        // GET /api/sucursal/productos/sucursal/:id
        // ============================================================

        [HttpGet("sucursal/{id}")]
        public IActionResult GetProductosPorSucursal(int id)
        {
            try
            {
                // Verificar si la sucursal existe
                var sucursalExiste = _context.Sucursales.Any(s => s.Id_Sucursal == id);

                if (!sucursalExiste)
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "Sucursal no encontrada o sin inventario activo"
                    });
                }

                var productos = (
                    from inv in _context.Inventarios
                    join p in _context.Productos on inv.Id_Producto equals p.Id_Producto
                    join c in _context.Categorias on p.Categoria equals c.Nombre
                    where inv.Id_Sucursal == id && p.Activo == true
                    select new
                    {
                        id_producto = p.Id_Producto,
                        nombre = p.Nombre,
                        categoria = c.Nombre,
                        precio = p.Precio,
                        existencia = inv.Existencia
                    }
                ).ToList();

                if (productos.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "Sucursal sin inventario activo"
                    });
                }

                return Ok(new
                {
                    status = "success",
                    data = productos
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error interno del servidor",

                });
            }
        }

        // ==================================================
        // GET /api/Sucursal/productos/Id


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var producto = _context.Productos
                    .Where(p => p.Id_Producto == id && p.Activo)
                    .Select(p => new
                    {
                        id_producto = p.Id_Producto,
                        nombre = p.Nombre,
                        categoria = p.Categoria,   // <-- nombre de la categoría tal como lo tienes
                        precio = p.Precio,
                        activo = p.Activo
                    })
                    .FirstOrDefault();

                // Si no existe
                if (producto == null)
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "Producto no encontrado"
                    });
                }

                // Si existe
                return Ok(new
                {
                    status = "success",
                    data = producto
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error interno del servidor",

                });
            }
        }


        // ==================================================
        // GET /api/sucursal/productos/Categoria/:categoria
        // ==================================================
        [HttpGet("categoria/{categoria}")]
        public IActionResult GetByCategoria(string categoria)
        {
            try
            {
                var productos = _context.Productos
                    .Where(p => p.Categoria == categoria && p.Activo)
                    .Select(p => new
                    {
                        id_producto = p.Id_Producto,
                        nombre = p.Nombre,
                        precio = p.Precio,
                        activo = p.Activo
                    })
                    .ToList();

                // Si no existen productos activos en esa categoría
                if (productos == null || productos.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "Categoría no encontrada o sin productos activos"
                    });
                }

                return Ok(new
                {
                    status = "success",
                    data = productos
                });
            }
            catch (Exception)
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

