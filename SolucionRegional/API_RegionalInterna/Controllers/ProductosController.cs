using Microsoft.AspNetCore.Mvc;
using API_RegionalInterna.Data;
using API_RegionalInterna.Models;

namespace API_RegionalInterna.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly RegionalDbContext _context;

        public ProductosController(RegionalDbContext context)
        {
            _context = context;
        }

        // GET: api/productos
        [HttpGet]
        public IActionResult GetAll()
        {
            var productos = _context.Productos.ToList();
            return Ok(new { status = "success", data = productos });
        }

        // GET: api/productos/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var producto = _context.Productos.Find(id);
            if (producto == null)
                return NotFound(new { status = "error", message = "Producto no encontrado" });

            return Ok(new { status = "success", data = producto });
        }

        // POST: api/productos
        [HttpPost]
        public IActionResult Create([FromBody] Producto nuevo)
        {
            if (string.IsNullOrEmpty(nuevo.Nombre) || nuevo.Precio <= 0)
                return BadRequest(new { status = "error", message = "Datos inválidos" });

            _context.Productos.Add(nuevo);
            _context.SaveChanges();

            return Created("", new
            {
                status = "success",
                message = "Producto creado correctamente",
                id_producto = nuevo.Id_Producto
            });
        }

        // PUT: api/productos/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Producto data)
        {
            var producto = _context.Productos.Find(id);
            if (producto == null)
                return NotFound(new { status = "error", message = "Producto no encontrado" });

            producto.Nombre = data.Nombre;
            producto.Categoria = data.Categoria;
            producto.Precio = data.Precio;
            producto.Existencia = data.Existencia;
            producto.Activo = data.Activo;

            _context.SaveChanges();

            return Ok(new { status = "success", message = "Producto actualizado correctamente" });
        }

        // PATCH: api/productos/existencia/{id}
        [HttpPatch("existencia/{id}")]
        public IActionResult UpdateExistencia(int id, [FromBody] dynamic body)
        {
            var producto = _context.Productos.Find(id);
            if (producto == null)
                return NotFound(new { status = "error", message = "Producto no encontrado" });

            producto.Existencia = (int)body.existencia;
            _context.SaveChanges();

            return Ok(new { status = "success", message = "Existencia actualizada correctamente" });
        }

        // DELETE: api/productos/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var producto = _context.Productos.Find(id);
            if (producto == null)
                return NotFound(new { status = "error", message = "Producto no encontrado" });

            producto.Activo = false;
            _context.SaveChanges();

            return Ok(new { status = "success", message = "Producto desactivado correctamente" });
        }
    }

}
