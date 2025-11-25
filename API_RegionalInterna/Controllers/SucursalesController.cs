using Microsoft.AspNetCore.Mvc;
using API_RegionalInterna.Data;
using API_RegionalInterna.Models;

namespace API_RegionalInterna.Controllers
{
    [ApiController]
    [Route("api/interna/sucursales")]
    public class SucursalesController : ControllerBase
    {
        private readonly RegionalDbContext _context;

        public SucursalesController(RegionalDbContext context)
        {
            _context = context;
        }

        // ✅ GET: /api/sucursales
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var sucursales = _context.Sucursales
                    .Where(s => s.Activa)
                    .Select(s => new
                    {
                        id_sucursal = s.Id_Sucursal,
                        nombre = s.Nombre,
                        direccion = s.Direccion,
                        telefono = s.Telefono,
                        activa = s.Activa ? 1 : 0
                    })
                    .ToList();

                // Si no hay sucursales activas — 404
                if (sucursales == null || sucursales.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "Sucursal no encontrada"
                    });
                }

                return Ok(new
                {
                    status = "success",
                    data = sucursales
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error interno del servidor"
                });
            }
        }


        // ✅ POST: /api/interna/sucursales
        [HttpPost]
        public IActionResult Create([FromBody] Sucursal sucursal)
        {
            // Activa siempre se fuerza a true en creación
            sucursal.Activa = true;

            _context.Sucursales.Add(sucursal);
            _context.SaveChanges();

            return StatusCode(201, new
            {
                status = "success",
                message = "Sucursal creada correctamente",
                id_sucursal = sucursal.Id_Sucursal
            });
        }

        // PUT: /api/interna/sucursales/id
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Sucursal input)
        {
            try
            {
                // Validación 400 - Datos inválidos o incompletos
                if (input == null ||
                    string.IsNullOrWhiteSpace(input.Nombre) ||
                    string.IsNullOrWhiteSpace(input.Direccion) ||
                    string.IsNullOrWhiteSpace(input.Telefono))
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = "Datos inválidos o incompletos"
                    });
                }

                var sucursal = _context.Sucursales.FirstOrDefault(s => s.Id_Sucursal == id);

                // Error 404 - No encontrada
                if (sucursal == null)
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "La sucursal no existe"
                    });
                }

                // Actualizar campos
                sucursal.Nombre = input.Nombre;
                sucursal.Direccion = input.Direccion;
                sucursal.Telefono = input.Telefono;

                _context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Sucursal actualizada correctamente",
                    data = new
                    {
                        id_sucursal = sucursal.Id_Sucursal,
                        nombre = sucursal.Nombre,
                        direccion = sucursal.Direccion,
                        telefono = sucursal.Telefono,
                        activa = sucursal.Activa ? 1 : 0
                    }
                });
            }
            catch (Exception)
            {
                // Error 500 - Error interno
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error interno del servidor"
                });
            }
        }
        // DELETE: /api/interna/sucursales/id
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var sucursal = _context.Sucursales.FirstOrDefault(s => s.Id_Sucursal == id);

                // Error 404 - No encontrada
                if (sucursal == null)
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "La sucursal no existe"
                    });
                }

                // Desactivar sucursal
                sucursal.Activa = false;
                _context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Sucursal desactivada correctamente"
                });
            }
            catch (Exception)
            {
                // Error 500 - Error interno
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error interno del servidor"
                });
            }
        }
    }
}