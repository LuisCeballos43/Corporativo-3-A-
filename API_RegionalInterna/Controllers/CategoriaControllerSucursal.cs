using Microsoft.AspNetCore.Mvc;
using API_RegionalInterna.Data;
using API_RegionalInterna.Models;

namespace API_RegionalInterna.Controllers
{
    [ApiController]
    [Route("api/Sucursal/categorias")]
    public class CategoriasControllerSucursal : ControllerBase
    {
        private readonly RegionalDbContext _context;

        public CategoriasControllerSucursal(RegionalDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // GET /api/Sucursal/categorias
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

                if (categorias == null || categorias.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "No existen categorías activas"
                    });
                }

                return Ok(new
                {
                    status = "success",
                    data = categorias
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
