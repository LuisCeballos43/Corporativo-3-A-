using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegionalDataBases.Data;
using RegionalDataBases.Models;
using RegionalDataBases.DTOs.Inventario;

namespace RegionalDataBases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== GET: Obtener todo el inventario ==========
        // URL: GET api/Inventario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventarioDto>>> GetInventario()
        {
            var inventario = await _context.Inventarios
                .Include(i => i.Sucursal)    // Cargar sucursal
                .Include(i => i.Producto)    // Cargar producto
                    .ThenInclude(p => p.Categoria)  // Y la categoría del producto
                .Select(i => new InventarioDto
                {
                    IdInventario = i.IdInventario,
                    IdSucursal = i.IdSucursal,
                    NombreSucursal = i.Sucursal.Nombre,
                    IdProducto = i.IdProducto,
                    NombreProducto = i.Producto.Nombre,
                    PrecioProducto = i.Producto.Precio,
                    Existencia = i.Existencia,
                    UltimaActualizacion = i.UltimaActualizacion
                })
                .ToListAsync();

            return Ok(inventario);
        }

        // ========== GET: Obtener inventario de una sucursal ==========
        // URL: GET api/Inventario/sucursal/5
        [HttpGet("sucursal/{idSucursal}")]
        public async Task<ActionResult<IEnumerable<InventarioDto>>> GetInventarioPorSucursal(int idSucursal)
        {
            // Verificar que la sucursal existe
            var sucursalExists = await _context.Sucursales.AnyAsync(s => s.IdSucursal == idSucursal);

            if (!sucursalExists)
            {
                return NotFound(new { message = "Sucursal no encontrada" });
            }

            var inventario = await _context.Inventarios
                .Include(i => i.Sucursal)
                .Include(i => i.Producto)
                    .ThenInclude(p => p.Categoria)
                .Where(i => i.IdSucursal == idSucursal)
                .Select(i => new InventarioDto
                {
                    IdInventario = i.IdInventario,
                    IdSucursal = i.IdSucursal,
                    NombreSucursal = i.Sucursal.Nombre,
                    IdProducto = i.IdProducto,
                    NombreProducto = i.Producto.Nombre,
                    PrecioProducto = i.Producto.Precio,
                    Existencia = i.Existencia,
                    UltimaActualizacion = i.UltimaActualizacion
                })
                .ToListAsync();

            return Ok(inventario);
        }

        // ========== GET: Obtener inventario de un producto ==========
        // URL: GET api/Inventario/producto/5
        [HttpGet("producto/{idProducto}")]
        public async Task<ActionResult<IEnumerable<InventarioDto>>> GetInventarioPorProducto(int idProducto)
        {
            // Verificar que el producto existe
            var productoExists = await _context.Productos.AnyAsync(p => p.IdProducto == idProducto);

            if (!productoExists)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            // Muestra en qué sucursales está ese producto
            var inventario = await _context.Inventarios
                .Include(i => i.Sucursal)
                .Include(i => i.Producto)
                .Where(i => i.IdProducto == idProducto)
                .Select(i => new InventarioDto
                {
                    IdInventario = i.IdInventario,
                    IdSucursal = i.IdSucursal,
                    NombreSucursal = i.Sucursal.Nombre,
                    IdProducto = i.IdProducto,
                    NombreProducto = i.Producto.Nombre,
                    PrecioProducto = i.Producto.Precio,
                    Existencia = i.Existencia,
                    UltimaActualizacion = i.UltimaActualizacion
                })
                .ToListAsync();

            return Ok(inventario);
        }

        // ========== GET: Obtener UN registro de inventario específico ==========
        // URL: GET api/Inventario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InventarioDetalleDto>> GetInventarioById(int id)
        {
            var inventario = await _context.Inventarios
                .Include(i => i.Sucursal)
                .Include(i => i.Producto)
                    .ThenInclude(p => p.Categoria)
                .Where(i => i.IdInventario == id)
                .Select(i => new InventarioDetalleDto
                {
                    IdInventario = i.IdInventario,
                    Existencia = i.Existencia,
                    UltimaActualizacion = i.UltimaActualizacion,
                    Sucursal = new SucursalInfoDto
                    {
                        IdSucursal = i.Sucursal.IdSucursal,
                        Nombre = i.Sucursal.Nombre,
                        Direccion = i.Sucursal.Direccion
                    },
                    Producto = new ProductoInfoDto
                    {
                        IdProducto = i.Producto.IdProducto,
                        Nombre = i.Producto.Nombre,
                        Precio = i.Producto.Precio,
                        Categoria = i.Producto.Categoria.NombreCategoria
                    }
                })
                .FirstOrDefaultAsync();

            if (inventario == null)
            {
                return NotFound(new { message = "Registro de inventario no encontrado" });
            }

            return Ok(inventario);
        }

        // ========== GET: Productos con bajo inventario ==========
        // URL: GET api/Inventario/bajo-stock?minimo=10
        [HttpGet("bajo-stock")]
        public async Task<ActionResult<IEnumerable<InventarioDto>>> GetInventarioBajoStock([FromQuery] int minimo = 10)
        {
            var inventario = await _context.Inventarios
                .Include(i => i.Sucursal)
                .Include(i => i.Producto)
                .Where(i => i.Existencia <= minimo)
                .Select(i => new InventarioDto
                {
                    IdInventario = i.IdInventario,
                    IdSucursal = i.IdSucursal,
                    NombreSucursal = i.Sucursal.Nombre,
                    IdProducto = i.IdProducto,
                    NombreProducto = i.Producto.Nombre,
                    PrecioProducto = i.Producto.Precio,
                    Existencia = i.Existencia,
                    UltimaActualizacion = i.UltimaActualizacion
                })
                .OrderBy(i => i.Existencia)  // Ordena por menor existencia
                .ToListAsync();

            return Ok(inventario);
        }

        // ========== POST: Crear registro de inventario ==========
        // URL: POST api/Inventario
        // Body: { "idSucursal": 1, "idProducto": 1, "existencia": 100 }
        [HttpPost]
        public async Task<ActionResult<InventarioDto>> CreateInventario(InventarioCreateDto inventarioDto)
        {
            // 1. Validar que la sucursal existe
            var sucursalExists = await _context.Sucursales
                .AnyAsync(s => s.IdSucursal == inventarioDto.IdSucursal);

            if (!sucursalExists)
            {
                return BadRequest(new { message = "La sucursal especificada no existe" });
            }

            // 2. Validar que el producto existe
            var productoExists = await _context.Productos
                .AnyAsync(p => p.IdProducto == inventarioDto.IdProducto);

            if (!productoExists)
            {
                return BadRequest(new { message = "El producto especificado no existe" });
            }

            // 3. Verificar que no exista ya ese producto en esa sucursal
            var inventarioExists = await _context.Inventarios
                .AnyAsync(i => i.IdSucursal == inventarioDto.IdSucursal
                            && i.IdProducto == inventarioDto.IdProducto);

            if (inventarioExists)
            {
                return BadRequest(new { message = "Ya existe un registro de inventario para este producto en esta sucursal" });
            }

            // 4. Validar existencia
            if (inventarioDto.Existencia < 0)
            {
                return BadRequest(new { message = "La existencia no puede ser negativa" });
            }

            // 5. Crear el inventario
            var inventario = new Inventario
            {
                IdSucursal = inventarioDto.IdSucursal,
                IdProducto = inventarioDto.IdProducto,
                Existencia = inventarioDto.Existencia,
                UltimaActualizacion = DateTime.Now
            };

            _context.Inventarios.Add(inventario);
            await _context.SaveChangesAsync();

            // 6. Obtener con relaciones para devolver
            var inventarioResponse = await _context.Inventarios
                .Include(i => i.Sucursal)
                .Include(i => i.Producto)
                .Where(i => i.IdInventario == inventario.IdInventario)
                .Select(i => new InventarioDto
                {
                    IdInventario = i.IdInventario,
                    IdSucursal = i.IdSucursal,
                    NombreSucursal = i.Sucursal.Nombre,
                    IdProducto = i.IdProducto,
                    NombreProducto = i.Producto.Nombre,
                    PrecioProducto = i.Producto.Precio,
                    Existencia = i.Existencia,
                    UltimaActualizacion = i.UltimaActualizacion
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetInventarioById), new { id = inventario.IdInventario }, inventarioResponse);
        }

        // ========== PUT: Actualizar existencia completa ==========
        // URL: PUT api/Inventario/5
        // Body: { "existencia": 150 }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventario(int id, InventarioUpdateDto inventarioDto)
        {
            var inventario = await _context.Inventarios.FindAsync(id);

            if (inventario == null)
            {
                return NotFound(new { message = "Registro de inventario no encontrado" });
            }

            if (inventarioDto.Existencia < 0)
            {
                return BadRequest(new { message = "La existencia no puede ser negativa" });
            }

            inventario.Existencia = inventarioDto.Existencia;
            inventario.UltimaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ========== PATCH: Ajustar existencias (agregar/quitar) ==========
        // URL: PATCH api/Inventario/5/ajustar
        // Body: { "cantidad": 50 }  (positivo para agregar, negativo para quitar)
        [HttpPatch("{id}/ajustar")]
        public async Task<ActionResult<InventarioDto>> AjustarInventario(int id, InventarioAjusteDto ajusteDto)
        {
            var inventario = await _context.Inventarios
                .Include(i => i.Sucursal)
                .Include(i => i.Producto)
                .FirstOrDefaultAsync(i => i.IdInventario == id);

            if (inventario == null)
            {
                return NotFound(new { message = "Registro de inventario no encontrado" });
            }

            // Calcular nueva existencia
            int nuevaExistencia = inventario.Existencia + ajusteDto.Cantidad;

            if (nuevaExistencia < 0)
            {
                return BadRequest(new { message = $"No hay suficiente inventario. Existencia actual: {inventario.Existencia}" });
            }

            inventario.Existencia = nuevaExistencia;
            inventario.UltimaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            var resultado = new InventarioDto
            {
                IdInventario = inventario.IdInventario,
                IdSucursal = inventario.IdSucursal,
                NombreSucursal = inventario.Sucursal.Nombre,
                IdProducto = inventario.IdProducto,
                NombreProducto = inventario.Producto.Nombre,
                PrecioProducto = inventario.Producto.Precio,
                Existencia = inventario.Existencia,
                UltimaActualizacion = inventario.UltimaActualizacion
            };

            return Ok(resultado);
        }

        // ========== DELETE: Eliminar registro de inventario ==========
        // URL: DELETE api/Inventario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventario(int id)
        {
            var inventario = await _context.Inventarios.FindAsync(id);

            if (inventario == null)
            {
                return NotFound(new { message = "Registro de inventario no encontrado" });
            }

            _context.Inventarios.Remove(inventario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}