using API_RegionalInterna.Data;
using API_RegionalInterna.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace API_RegionalInterna.Controllers
{
    [ApiController]
    [Route("api/interna/productos")]
    public class ProductosController : ControllerBase
    {
        private readonly RegionalDbContext _context;

        public ProductosController(RegionalDbContext context)
        {
            _context = context;
        }

        // ==================================================
        // GET /api/interna/productos
        // ==================================================
        [HttpGet]
        public IActionResult GetAll()
        {
            var productos = _context.Productos
                .Where(p => p.Activo)
                .Select(p => new
                {
                    id_producto = p.Id_Producto,
                    nombre = p.Nombre,
                    categoria = p.Categoria,
                    precio = p.Precio,
                    existencia = p.Existencia
                })
                .ToList();

            if (!productos.Any())
            {
                return NotFound(new
                {
                    status = "error",
                    message = "Producto no encontrado"
                });
            }

            return Ok(new { status = "success", data = productos });
        }

        // ==================================================
        // GET /api/interna/productos/{id}
        // ==================================================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var p = _context.Productos
                .FirstOrDefault(x => x.Id_Producto == id && x.Activo);

            if (p == null)
            {
                return NotFound(new
                {
                    status = "error",
                    message = "Producto no encontrado"
                });
            }

            return Ok(new
            {
                status = "success",
                data = new
                {
                    id_producto = p.Id_Producto,
                    nombre = p.Nombre,
                    categoria = p.Categoria,
                    precio = p.Precio,
                    existencia = p.Existencia
                }
            });
        }

        // ==================================================
        // POST /api/productos
        // ==================================================
        [HttpPost]
        public IActionResult Create([FromBody] Producto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Datos inválidos o incompletos"
                });
            }

            _context.Productos.Add(model);
            _context.SaveChanges();

            return StatusCode(201, new
            {
                status = "success",
                message = "Producto creado correctamente",
                id_producto = model.Id_Producto
            });
        }

        // ==================================================
        // PUT /api/interna/productos/{id}
        // ==================================================
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Producto model)
        {
            var p = _context.Productos.FirstOrDefault(x => x.Id_Producto == id);

            if (p == null)
            {
                return NotFound(new
                {
                    status = "error",
                    message = "Producto no encontrado"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Datos inválidos o incompletos"
                });
            }

            p.Nombre = model.Nombre;
            p.Categoria = model.Categoria;
            p.Precio = model.Precio;
            p.Existencia = model.Existencia;
            p.Activo = model.Activo;

            _context.SaveChanges();

            return Ok(new
            {
                status = "success",
                message = "Producto actualizado correctamente"
            });
        }

        // ==================================================
        // PATCH /api/interna/productos/existencia/{id}
        // ==================================================
        [HttpPatch("existencia/{id}")]
        public IActionResult UpdateExistencia(int id, [FromBody] JsonElement body)
        {
            if (!body.TryGetProperty("existencia", out JsonElement existenciaProp))
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Datos inválidos o incompletos"
                });
            }

            int nuevaExistencia = existenciaProp.GetInt32();

            var producto = _context.Productos.FirstOrDefault(x => x.Id_Producto == id);

            if (producto == null)
            {
                return NotFound(new
                {
                    status = "error",
                    message = "Producto no encontrado"
                });
            }

            producto.Existencia = nuevaExistencia;
            _context.SaveChanges();

            return Ok(new
            {
                status = "success",
                message = "Producto actualizado correctamente"
            });
        }


        // ==================================================
        // DELETE /api/interna/productos/{id}
        // ==================================================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var p = _context.Productos.FirstOrDefault(x => x.Id_Producto == id);

            if (p == null)
            {
                return NotFound(new
                {
                    status = "error",
                    message = "Producto no encontrado"
                });
            }

            p.Activo = false;
            _context.SaveChanges();

            return Ok(new
            {
                status = "success",
                message = "Producto desactivado correctamente"
            });
        }
    }
}
