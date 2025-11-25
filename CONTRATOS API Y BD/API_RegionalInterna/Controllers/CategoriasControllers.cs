using Microsoft.AspNetCore.Mvc;
using API_RegionalInterna.Data;
using API_RegionalInterna.Models;

namespace API_RegionalInterna.Controllers
{
    [ApiController]
    [Route("api/interna/categoria")]
    public class CategoriasController : ControllerBase
    {
        private readonly RegionalDbContext _context;

        public CategoriasController(RegionalDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // GET /api/interna/categorias
        // ============================================================
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var categorias = _context.Categorias
                    .Where(c => c.Activa)
                    .Select(c => new
                    {
                        id_categoria = c.Id_Categoria,
                        categoria = c.Nombre
                    })
                    .ToList();

                return Ok(new
                {
                    status = "success",
                    data = categorias
                });
            }
            catch
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error interno del servidor"
                });
            }
        }

        // ============================================================
        // POST /api/interna/categorias
        // ============================================================
        [HttpPost]
        public IActionResult Create([FromBody] Categoria categoria)
        {
            try
            {
                if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = "Datos inválidos o incompletos"
                    });
                }

                categoria.Activa = true;

                _context.Categorias.Add(categoria);
                _context.SaveChanges();

                return StatusCode(201, new
                {
                    status = "success",
                    message = "Categoría creada correctamente",
                    id_categoria = categoria.Id_Categoria
                });
            }
            catch
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error interno del servidor"
                });
            }
        }

        // ============================================================
        // PUT /api/interna/categorias/{id}
        // ============================================================
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Categoria body)
        {
            try
            {
                var categoria = _context.Categorias.FirstOrDefault(c => c.Id_Categoria == id);

                if (categoria == null)
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "Categoría no encontrada"
                    });
                }

                categoria.Nombre = body.Nombre;
                _context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Categoría actualizada correctamente"
                });
            }
            catch
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error interno del servidor"
                });
            }
        }

        // ============================================================
        // DELETE /api/interna/categorias/{id}
        // ============================================================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var categoria = _context.Categorias.FirstOrDefault(c => c.Id_Categoria == id);

                if (categoria == null)
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "Categoría no encontrada"
                    });
                }

                categoria.Activa = false; // Soft delete
                _context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Categoría desactivada correctamente"
                });
            }
            catch
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error interno del servidor"
                });
            }
        }
    }
}
