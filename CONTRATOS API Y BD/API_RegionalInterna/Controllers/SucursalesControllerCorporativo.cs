using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_RegionalInterna.Data;
using API_RegionalInterna.Models;

namespace API_RegionalInterna.Controllers
{
    [ApiController]
    [Route("api/corporativo/sucursales")]
    public class SucursalesControllerCorporativo : ControllerBase
    {
        private readonly RegionalDbContext _context;
        public SucursalesControllerCorporativo(RegionalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllSucursales()
        {
            try
            {
                var sucursales = _context.Sucursales
                    .Select(s => new
                    {
                        id_sucursal = s.Id_Sucursal,
                        nombre = s.Nombre,
                        direccion = s.Direccion,
                        telefono = s.Telefono,
                        activa = s.Activa
                    })
                    .ToList();

                if (!sucursales.Any())
                {
                    return NotFound(new { status = "error", message = "No existen sucursales registradas" });
                }

                return Ok(new { status = "success", data = sucursales });
            }
            catch (Exception)
            {
                return StatusCode(500, new { status = "error", message = "Error interno del servidor" });
            }
        }

        //api/corporativo/sucursales/id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSucursalById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "El ID debe ser un número mayor a 0."
                });
            }

            try
            {
                var sucursal = await _context.Sucursales
                    .Where(s => s.Id_Sucursal == id)
                    .Select(s => new
                    {
                        id_sucursal = s.Id_Sucursal,
                        nombre = s.Nombre,
                        direccion = s.Direccion,
                        telefono = s.Telefono,
                        activa = s.Activa
                    })
                    .FirstOrDefaultAsync();

                if (sucursal == null)
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
                    data = sucursal
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


    }
}

