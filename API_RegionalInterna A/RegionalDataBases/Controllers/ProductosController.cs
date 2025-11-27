using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegionalDataBases.Data;
using RegionalDataBases.Models;
using RegionalDataBases.DTOs.Productos;

namespace RegionalDataBases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== GET: Obtener todos los productos ==========
        // URL: GET api/Productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
        {
            // Include() carga la relación con Categoria
            var productos = await _context.Productos
                .Include(p => p.Categoria)  // ← Carga la categoría relacionada
                .Select(p => new ProductoDto
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    IdCategoria = p.IdCategoria,
                    NombreCategoria = p.Categoria.NombreCategoria,  // ← De la relación
                    Precio = p.Precio,
                    Activo = p.Activo
                })
                .ToListAsync();

            return Ok(productos);
        }

        // ========== GET: Obtener UN producto por ID ==========
        // URL: GET api/Productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDetalleDto>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.IdProducto == id)
                .Select(p => new ProductoDetalleDto
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Activo = p.Activo,
                    Categoria = new CategoriaSimpleDto
                    {
                        IdCategoria = p.Categoria.IdCategoria,
                        Nombre = p.Categoria.NombreCategoria
                    }
                })
                .FirstOrDefaultAsync();

            if (producto == null)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            return Ok(producto);
        }

        // ========== GET: Obtener productos por categoría ==========
        // URL: GET api/Productos/categoria/5
        [HttpGet("categoria/{idCategoria}")]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductosPorCategoria(int idCategoria)
        {
            // Verificar que la categoría existe
            var categoriaExists = await _context.Categorias.AnyAsync(c => c.IdCategoria == idCategoria);

            if (!categoriaExists)
            {
                return NotFound(new { message = "Categoría no encontrada" });
            }

            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.IdCategoria == idCategoria)  // ← Filtrar por categoría
                .Select(p => new ProductoDto
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    IdCategoria = p.IdCategoria,
                    NombreCategoria = p.Categoria.NombreCategoria,
                    Precio = p.Precio,
                    Activo = p.Activo
                })
                .ToListAsync();

            return Ok(productos);
        }

        // ========== GET: Obtener solo productos activos ==========
        // URL: GET api/Productos/activos
        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductosActivos()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Activo == true)
                .Select(p => new ProductoDto
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    IdCategoria = p.IdCategoria,
                    NombreCategoria = p.Categoria.NombreCategoria,
                    Precio = p.Precio,
                    Activo = p.Activo
                })
                .ToListAsync();

            return Ok(productos);
        }

        // ========== GET: Buscar productos por nombre ==========
        // URL: GET api/Productos/buscar?nombre=laptop
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> BuscarProductos([FromQuery] string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest(new { message = "Debe proporcionar un nombre para buscar" });
            }

            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Nombre.Contains(nombre))  // ← Busca que contenga el texto
                .Select(p => new ProductoDto
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    IdCategoria = p.IdCategoria,
                    NombreCategoria = p.Categoria.NombreCategoria,
                    Precio = p.Precio,
                    Activo = p.Activo
                })
                .ToListAsync();

            return Ok(productos);
        }

        // ========== POST: Crear nuevo producto ==========
        // URL: POST api/Productos
        // Body: { "nombre": "Laptop HP", "idCategoria": 1, "precio": 15000, "activo": true }
        [HttpPost]
        public async Task<ActionResult<ProductoDto>> CreateProducto(ProductoCreateDto productoDto)
        {
            // 1. Validar que la categoría existe
            var categoriaExists = await _context.Categorias
                .AnyAsync(c => c.IdCategoria == productoDto.IdCategoria);

            if (!categoriaExists)
            {
                return BadRequest(new { message = "La categoría especificada no existe" });
            }

            // 2. Validar que el precio sea positivo
            if (productoDto.Precio <= 0)
            {
                return BadRequest(new { message = "El precio debe ser mayor a 0" });
            }

            // 3. Crear el producto
            var producto = new Producto
            {
                Nombre = productoDto.Nombre,
                IdCategoria = productoDto.IdCategoria,
                Precio = productoDto.Precio,
                Activo = productoDto.Activo
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // 4. Obtener el producto con la categoría para devolver
            var productoResponse = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.IdProducto == producto.IdProducto)
                .Select(p => new ProductoDto
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    IdCategoria = p.IdCategoria,
                    NombreCategoria = p.Categoria.NombreCategoria,
                    Precio = p.Precio,
                    Activo = p.Activo
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetProducto), new { id = producto.IdProducto }, productoResponse);
        }

        // ========== PUT: Actualizar producto ==========
        // URL: PUT api/Productos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(int id, ProductoUpdateDto productoDto)
        {
            // 1. Buscar el producto
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            // 2. Validar que la categoría existe
            var categoriaExists = await _context.Categorias
                .AnyAsync(c => c.IdCategoria == productoDto.IdCategoria);

            if (!categoriaExists)
            {
                return BadRequest(new { message = "La categoría especificada no existe" });
            }

            // 3. Validar precio
            if (productoDto.Precio <= 0)
            {
                return BadRequest(new { message = "El precio debe ser mayor a 0" });
            }

            // 4. Actualizar
            producto.Nombre = productoDto.Nombre;
            producto.IdCategoria = productoDto.IdCategoria;
            producto.Precio = productoDto.Precio;
            producto.Activo = productoDto.Activo;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProductoExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // ========== DELETE: Eliminar producto ==========
        // URL: DELETE api/Productos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            // Verificar si está en inventario
            var estaEnInventario = await _context.Inventarios
                .AnyAsync(i => i.IdProducto == id);

            if (estaEnInventario)
            {
                return BadRequest(new { message = "No se puede eliminar el producto porque está en inventario" });
            }

            // Verificar si está en detalles de venta
            var estaEnVentas = await _context.DetallesVenta
                .AnyAsync(d => d.IdProducto == id);

            if (estaEnVentas)
            {
                return BadRequest(new { message = "No se puede eliminar el producto porque tiene ventas registradas" });
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> ProductoExists(int id)
        {
            return await _context.Productos.AnyAsync(e => e.IdProducto == id);
        }
    }
}