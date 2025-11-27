using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegionalDataBases.Data;
using RegionalDataBases.Models;
using RegionalDataBases.DTOs.Clientes;
using System.Text.RegularExpressions;

namespace RegionalDataBases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== GET: Obtener todos los clientes ==========
        // URL: GET api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
        {
            var clientes = await _context.Clientes
                .Include(c => c.Sucursal)  // ← Carga la sucursal relacionada
                .Select(c => new ClienteDto
                {
                    IdCliente = c.IdCliente,
                    Nombre = c.Nombre,
                    Telefono = c.Telefono,
                    Direccion = c.Direccion,
                    Email = c.Email,
                    IdSucursal = c.IdSucursal,
                    NombreSucursal = c.Sucursal.Nombre,
                    Activo = c.Activo
                })
                .ToListAsync();

            return Ok(clientes);
        }

        // ========== GET: Obtener UN cliente por ID ==========
        // URL: GET api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDetalleDto>> GetCliente(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Sucursal)
                .Where(c => c.IdCliente == id)
                .Select(c => new ClienteDetalleDto
                {
                    IdCliente = c.IdCliente,
                    Nombre = c.Nombre,
                    Telefono = c.Telefono,
                    Direccion = c.Direccion,
                    Email = c.Email,
                    Activo = c.Activo,
                    Sucursal = new SucursalSimpleDto
                    {
                        IdSucursal = c.Sucursal.IdSucursal,
                        Nombre = c.Sucursal.Nombre,
                        Telefono = c.Sucursal.Telefono
                    }
                })
                .FirstOrDefaultAsync();

            if (cliente == null)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }

            return Ok(cliente);
        }

        // ========== GET: Obtener clientes por sucursal ==========
        // URL: GET api/Clientes/sucursal/5
        [HttpGet("sucursal/{idSucursal}")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientesPorSucursal(int idSucursal)
        {
            // Verificar que la sucursal existe
            var sucursalExists = await _context.Sucursales.AnyAsync(s => s.IdSucursal == idSucursal);

            if (!sucursalExists)
            {
                return NotFound(new { message = "Sucursal no encontrada" });
            }

            var clientes = await _context.Clientes
                .Include(c => c.Sucursal)
                .Where(c => c.IdSucursal == idSucursal)
                .Select(c => new ClienteDto
                {
                    IdCliente = c.IdCliente,
                    Nombre = c.Nombre,
                    Telefono = c.Telefono,
                    Direccion = c.Direccion,
                    Email = c.Email,
                    IdSucursal = c.IdSucursal,
                    NombreSucursal = c.Sucursal.Nombre,
                    Activo = c.Activo
                })
                .ToListAsync();

            return Ok(clientes);
        }

        // ========== GET: Obtener solo clientes activos ==========
        // URL: GET api/Clientes/activos
        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientesActivos()
        {
            var clientes = await _context.Clientes
                .Include(c => c.Sucursal)
                .Where(c => c.Activo == true)
                .Select(c => new ClienteDto
                {
                    IdCliente = c.IdCliente,
                    Nombre = c.Nombre,
                    Telefono = c.Telefono,
                    Direccion = c.Direccion,
                    Email = c.Email,
                    IdSucursal = c.IdSucursal,
                    NombreSucursal = c.Sucursal.Nombre,
                    Activo = c.Activo
                })
                .ToListAsync();

            return Ok(clientes);
        }

        // ========== GET: Buscar clientes por nombre ==========
        // URL: GET api/Clientes/buscar?nombre=juan
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> BuscarClientes([FromQuery] string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest(new { message = "Debe proporcionar un nombre para buscar" });
            }

            var clientes = await _context.Clientes
                .Include(c => c.Sucursal)
                .Where(c => c.Nombre.Contains(nombre))
                .Select(c => new ClienteDto
                {
                    IdCliente = c.IdCliente,
                    Nombre = c.Nombre,
                    Telefono = c.Telefono,
                    Direccion = c.Direccion,
                    Email = c.Email,
                    IdSucursal = c.IdSucursal,
                    NombreSucursal = c.Sucursal.Nombre,
                    Activo = c.Activo
                })
                .ToListAsync();

            return Ok(clientes);
        }

        // ========== POST: Crear nuevo cliente ==========
        // URL: POST api/Clientes
        // Body: { "nombre": "Juan Pérez", "telefono": "3121234567", "direccion": "Calle 123", "email": "juan@mail.com", "idSucursal": 1, "activo": true }
        [HttpPost]
        public async Task<ActionResult<ClienteDto>> CreateCliente(ClienteCreateDto clienteDto)
        {
            // 1. Validar que la sucursal existe
            var sucursalExists = await _context.Sucursales
                .AnyAsync(s => s.IdSucursal == clienteDto.IdSucursal);

            if (!sucursalExists)
            {
                return BadRequest(new { message = "La sucursal especificada no existe" });
            }

            // 2. Validar teléfono (10 dígitos)
            if (clienteDto.Telefono.Length != 10 || !clienteDto.Telefono.All(char.IsDigit))
            {
                return BadRequest(new { message = "El teléfono debe tener exactamente 10 dígitos" });
            }

            // 3. Validar email
            if (!IsValidEmail(clienteDto.Email))
            {
                return BadRequest(new { message = "El formato del email no es válido" });
            }

            // 4. Verificar que el email no esté registrado
            var emailExists = await _context.Clientes
                .AnyAsync(c => c.Email == clienteDto.Email);

            if (emailExists)
            {
                return BadRequest(new { message = "El email ya está registrado" });
            }

            // 5. Crear el cliente
            var cliente = new Cliente
            {
                Nombre = clienteDto.Nombre,
                Telefono = clienteDto.Telefono,
                Direccion = clienteDto.Direccion,
                Email = clienteDto.Email,
                IdSucursal = clienteDto.IdSucursal,
                Activo = clienteDto.Activo
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            // 6. Obtener el cliente con la sucursal para devolver
            var clienteResponse = await _context.Clientes
                .Include(c => c.Sucursal)
                .Where(c => c.IdCliente == cliente.IdCliente)
                .Select(c => new ClienteDto
                {
                    IdCliente = c.IdCliente,
                    Nombre = c.Nombre,
                    Telefono = c.Telefono,
                    Direccion = c.Direccion,
                    Email = c.Email,
                    IdSucursal = c.IdSucursal,
                    NombreSucursal = c.Sucursal.Nombre,
                    Activo = c.Activo
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.IdCliente }, clienteResponse);
        }

        // ========== PUT: Actualizar cliente ==========
        // URL: PUT api/Clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, ClienteUpdateDto clienteDto)
        {
            // 1. Buscar el cliente
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }

            // 2. Validar que la sucursal existe
            var sucursalExists = await _context.Sucursales
                .AnyAsync(s => s.IdSucursal == clienteDto.IdSucursal);

            if (!sucursalExists)
            {
                return BadRequest(new { message = "La sucursal especificada no existe" });
            }

            // 3. Validar teléfono
            if (clienteDto.Telefono.Length != 10 || !clienteDto.Telefono.All(char.IsDigit))
            {
                return BadRequest(new { message = "El teléfono debe tener exactamente 10 dígitos" });
            }

            // 4. Validar email
            if (!IsValidEmail(clienteDto.Email))
            {
                return BadRequest(new { message = "El formato del email no es válido" });
            }

            // 5. Verificar que el email no esté usado por otro cliente
            var emailExists = await _context.Clientes
                .AnyAsync(c => c.Email == clienteDto.Email && c.IdCliente != id);

            if (emailExists)
            {
                return BadRequest(new { message = "El email ya está registrado por otro cliente" });
            }

            // 6. Actualizar
            cliente.Nombre = clienteDto.Nombre;
            cliente.Telefono = clienteDto.Telefono;
            cliente.Direccion = clienteDto.Direccion;
            cliente.Email = clienteDto.Email;
            cliente.IdSucursal = clienteDto.IdSucursal;
            cliente.Activo = clienteDto.Activo;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ClienteExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // ========== DELETE: Eliminar cliente ==========
        // URL: DELETE api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }

            // Verificar si tiene ventas registradas
            var tieneVentas = await _context.ReportesVenta
                .AnyAsync(r => r.IdCliente == id);

            if (tieneVentas)
            {
                return BadRequest(new { message = "No se puede eliminar el cliente porque tiene ventas registradas" });
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ========== Métodos auxiliares privados ==========

        private async Task<bool> ClienteExists(int id)
        {
            return await _context.Clientes.AnyAsync(e => e.IdCliente == id);
        }

        // Validar formato de email
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Regex simple para validar email
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }
}