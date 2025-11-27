using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegionalDataBases.Data;
using RegionalDataBases.Models;
using RegionalDataBases.DTOs.Sucursales;

namespace RegionalDataBases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SucursalesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SucursalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== GET: Obtener todas las sucursales ==========
        // URL: GET api/Sucursales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SucursalDto>>> GetSucursales()
        {
            var sucursales = await _context.Sucursales
                .Select(s => new SucursalDto
                {
                    IdSucursal = s.IdSucursal,
                    Nombre = s.Nombre,
                    Direccion = s.Direccion,
                    Telefono = s.Telefono,
                    Activa = s.Activa
                })
                .ToListAsync();

            return Ok(sucursales);
        }

        // ========== GET: Obtener UNA sucursal por ID ==========
        // URL: GET api/Sucursales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SucursalDto>> GetSucursal(int id)
        {
            var sucursal = await _context.Sucursales
                .Where(s => s.IdSucursal == id)
                .Select(s => new SucursalDto
                {
                    IdSucursal = s.IdSucursal,
                    Nombre = s.Nombre,
                    Direccion = s.Direccion,
                    Telefono = s.Telefono,
                    Activa = s.Activa
                })
                .FirstOrDefaultAsync();

            if (sucursal == null)
            {
                return NotFound(new { message = "Sucursal no encontrada" });
            }

            return Ok(sucursal);
        }

        // ========== GET: Obtener solo sucursales activas ==========
        // URL: GET api/Sucursales/activas
        // IMPORTANTE: Este endpoint va ANTES del {id} para evitar conflictos
        [HttpGet("activas")]
        public async Task<ActionResult<IEnumerable<SucursalDto>>> GetSucursalesActivas()
        {
            var sucursales = await _context.Sucursales
                .Where(s => s.Activa == true)
                .Select(s => new SucursalDto
                {
                    IdSucursal = s.IdSucursal,
                    Nombre = s.Nombre,
                    Direccion = s.Direccion,
                    Telefono = s.Telefono,
                    Activa = s.Activa
                })
                .ToListAsync();

            return Ok(sucursales);
        }

        // ========== POST: Crear nueva sucursal ==========
        // URL: POST api/Sucursales
        // Body: { "nombre": "Sucursal Centro", "direccion": "Av. Principal 123", "telefono": "3121234567", "activa": true }
        [HttpPost]
        public async Task<ActionResult<SucursalDto>> CreateSucursal(SucursalCreateDto sucursalDto)
        {
            // Validar que el teléfono tenga 10 dígitos
            if (sucursalDto.Telefono.Length != 10)
            {
                return BadRequest(new { message = "El teléfono debe tener 10 dígitos" });
            }

            var sucursal = new Sucursal
            {
                Nombre = sucursalDto.Nombre,
                Direccion = sucursalDto.Direccion,
                Telefono = sucursalDto.Telefono,
                Activa = sucursalDto.Activa
            };

            _context.Sucursales.Add(sucursal);
            await _context.SaveChangesAsync();

            var sucursalResponse = new SucursalDto
            {
                IdSucursal = sucursal.IdSucursal,
                Nombre = sucursal.Nombre,
                Direccion = sucursal.Direccion,
                Telefono = sucursal.Telefono,
                Activa = sucursal.Activa
            };

            return CreatedAtAction(nameof(GetSucursal), new { id = sucursal.IdSucursal }, sucursalResponse);
        }

        // ========== PUT: Actualizar sucursal ==========
        // URL: PUT api/Sucursales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSucursal(int id, SucursalUpdateDto sucursalDto)
        {
            var sucursal = await _context.Sucursales.FindAsync(id);

            if (sucursal == null)
            {
                return NotFound(new { message = "Sucursal no encontrada" });
            }

            // Validar teléfono
            if (sucursalDto.Telefono.Length != 10)
            {
                return BadRequest(new { message = "El teléfono debe tener 10 dígitos" });
            }

            sucursal.Nombre = sucursalDto.Nombre;
            sucursal.Direccion = sucursalDto.Direccion;
            sucursal.Telefono = sucursalDto.Telefono;
            sucursal.Activa = sucursalDto.Activa;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SucursalExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // ========== DELETE: Eliminar sucursal ==========
        // URL: DELETE api/Sucursales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSucursal(int id)
        {
            var sucursal = await _context.Sucursales.FindAsync(id);

            if (sucursal == null)
            {
                return NotFound(new { message = "Sucursal no encontrada" });
            }

            // Verificar si tiene inventario asociado
            var tieneInventario = await _context.Inventarios
                .AnyAsync(i => i.IdSucursal == id);

            if (tieneInventario)
            {
                return BadRequest(new { message = "No se puede eliminar la sucursal porque tiene inventario asociado" });
            }

            // Verificar si tiene clientes asociados
            var tieneClientes = await _context.Clientes
                .AnyAsync(c => c.IdSucursal == id);

            if (tieneClientes)
            {
                return BadRequest(new { message = "No se puede eliminar la sucursal porque tiene clientes asociados" });
            }

            _context.Sucursales.Remove(sucursal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> SucursalExists(int id)
        {
            return await _context.Sucursales.AnyAsync(e => e.IdSucursal == id);
        }
    }
}