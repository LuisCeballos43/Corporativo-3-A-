using Microsoft.AspNetCore.Mvc;
using API_RegionalInterna.Data;
using API_RegionalInterna.Models;

namespace API_RegionalInterna.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly RegionalDbContext _context;

        public CategoriasController(RegionalDbContext context)
        {
            _context = context;
        }

        // ✅ GET: /api/categorias
        [HttpGet]
        public IActionResult GetAll()
        {
            var categorias = _context.Categorias
                                     .Where(c => c.Activo)
                                     .Select(c => new
                                     {
                                         id_categoria = c.Id_Categoria,
                                         categoria = c.Categoria_Nombre
                                     })
                                     .ToList();

            return Ok(new { status = "success", data = categorias });
        }

        // ✅ POST: /api/categorias
        [HttpPost]
        public IActionResult Create([FromBody] Categoria nueva)
        {
            if (string.IsNullOrWhiteSpace(nueva.Categoria_Nombre))
                return BadRequest(new { status = "error", message = "Datos inválidos o incompletos" });

            _context.Categorias.Add(nueva);
            _context.SaveChanges();

            return Created("", new
            {
                status = "success",
                message = "Categoría creada correctamente",
                id_categoria = nueva.Id_Categoria
            });
        }

        // ✅ PUT: /api/categorias/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Categoria data)
        {
            var categoria = _context.Categorias.Find(id);
            if (categoria == null)
                return NotFound(new { status = "error", message = "Categoría no encontrada" });

            if (string.IsNullOrWhiteSpace(data.Categoria_Nombre))
                return BadRequest(new { status = "error", message = "Datos inválidos o incompletos" });

            categoria.Categoria_Nombre = data.Categoria_Nombre;
            _context.SaveChanges();

            return Ok(new { status = "success", message = "Categoría actualizada correctamente" });
        }

        // ✅ DELETE: /api/categorias/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var categoria = _context.Categorias.Find(id);
            if (categoria == null)
                return NotFound(new { status = "error", message = "Categoría no encontrada" });

            categoria.Activo = false;
            _context.SaveChanges();

            return Ok(new { status = "success", message = "Categoría desactivada correctamente" });
        }
    }
}

