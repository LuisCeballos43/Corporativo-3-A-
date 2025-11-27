using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegionalDataBases.Data;
using RegionalDataBases.Models;
using RegionalDataBases.DTOs.Categorias;

namespace RegionalDataBases.Controllers
{
    [Route("api/[controller]")]  // URL será: api/Categorias
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        // Inyección de dependencias del contexto de BD
        private readonly ApplicationDbContext _context;

        public CategoriasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== GET: Obtener todas las categorías ==========
        // URL: GET api/Categorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
        {
            // 1. Consulta la tabla CATEGORIAS
            var categorias = await _context.Categorias
                // 2. Convierte cada Categoria (Model) a CategoriaDto
                .Select(c => new CategoriaDto
                {
                    IdCategoria = c.IdCategoria,
                    Categoria = c.NombreCategoria,
                    Activo = c.Activo
                })
                // 3. Ejecuta la consulta y devuelve lista
                .ToListAsync();

            // 4. Responde con código 200 OK y los datos en JSON
            return Ok(categorias);
        }

        // ========== GET: Obtener UNA categoría por ID ==========
        // URL: GET api/Categorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
        {
            // 1. Busca la categoría con ese ID
            var categoria = await _context.Categorias
                .Where(c => c.IdCategoria == id)
                .Select(c => new CategoriaDto
                {
                    IdCategoria = c.IdCategoria,
                    Categoria = c.NombreCategoria,
                    Activo = c.Activo
                })
                .FirstOrDefaultAsync();  // Trae la primera o null

            // 2. Si no existe, devuelve 404 Not Found
            if (categoria == null)
            {
                return NotFound(new { message = "Categoría no encontrada" });
            }

            // 3. Si existe, devuelve 200 OK con los datos
            return Ok(categoria);
        }

        // ========== POST: Crear nueva categoría ==========
        // URL: POST api/Categorias
        // Body: { "categoria": "Electrónica", "activo": true }
        [HttpPost]
        public async Task<ActionResult<CategoriaDto>> CreateCategoria(CategoriaCreateDto categoriaDto)
        {
            // 1. Convierte el DTO a un Model (entidad)
            var categoria = new Categoria
            {
                NombreCategoria = categoriaDto.Categoria,
                Activo = categoriaDto.Activo
            };

            // 2. Agrega a la BD (aún no se guarda)
            _context.Categorias.Add(categoria);

            // 3. Guarda los cambios en la BD (aquí se genera el ID)
            await _context.SaveChangesAsync();

            // 4. Prepara la respuesta con el nuevo registro
            var categoriaResponse = new CategoriaDto
            {
                IdCategoria = categoria.IdCategoria,  // Ya tiene ID generado
                Categoria = categoria.NombreCategoria,
                Activo = categoria.Activo
            };

            // 5. Devuelve 201 Created con la ubicación del nuevo recurso
            return CreatedAtAction(
                nameof(GetCategoria),  // Nombre del método GET
                new { id = categoria.IdCategoria },  // Parámetro para GET
                categoriaResponse  // El objeto creado
            );
        }

        // ========== PUT: Actualizar categoría existente ==========
        // URL: PUT api/Categorias/5
        // Body: { "categoria": "Electrónica Modificada", "activo": false }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoria(int id, CategoriaUpdateDto categoriaDto)
        {
            // 1. Busca la categoría en la BD
            var categoria = await _context.Categorias.FindAsync(id);

            // 2. Si no existe, devuelve 404
            if (categoria == null)
            {
                return NotFound(new { message = "Categoría no encontrada" });
            }

            // 3. Actualiza las propiedades
            categoria.NombreCategoria = categoriaDto.Categoria;
            categoria.Activo = categoriaDto.Activo;

            // 4. Intenta guardar los cambios
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si otro usuario modificó al mismo tiempo
                if (!await CategoriaExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            // 5. Devuelve 204 No Content (éxito sin datos)
            return NoContent();
        }

        // ========== DELETE: Eliminar categoría ==========
        // URL: DELETE api/Categorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            // 1. Busca la categoría
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound(new { message = "Categoría no encontrada" });
            }

            // 2. Verifica si tiene productos asociados
            var tieneProductos = await _context.Productos
                .AnyAsync(p => p.IdCategoria == id);

            if (tieneProductos)
            {
                return BadRequest(new { message = "No se puede eliminar la categoría porque tiene productos asociados" });
            }

            // 3. Elimina la categoría
            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            // 4. Devuelve 204 No Content
            return NoContent();
        }

        // ========== Método auxiliar privado ==========
        private async Task<bool> CategoriaExists(int id)
        {
            return await _context.Categorias.AnyAsync(e => e.IdCategoria == id);
        }
    }
}