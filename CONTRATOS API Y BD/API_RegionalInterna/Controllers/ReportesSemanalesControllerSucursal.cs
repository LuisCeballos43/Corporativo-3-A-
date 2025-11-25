using API_RegionalInterna.Data;
using API_RegionalInterna.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace API_RegionalInterna.Controllers
{
    [ApiController]
    [Route("api/sucursal/reportes-semanales")]
    public class ReportesSemanalesControllerSucursal : ControllerBase
    {
        private readonly RegionalDbContext _context;

        public ReportesSemanalesControllerSucursal(RegionalDbContext context)
        {
            _context = context;
        }

        // ================================
        // POST: /api/sucursal/reportes-semanales
        // ================================
        [HttpPost]
        public async Task<IActionResult> CrearReporte([FromBody] ReportesVentas modelo)
        {
            try
            {
                if (modelo == null ||
                    modelo.Id_Sucursal <= 0 ||
                    modelo.Año <= 0 ||
                    modelo.Mes <= 0 ||
                    modelo.Total_Ventas <= 0 ||
                    modelo.Id_Cliente <= 0 ||
                    modelo.Detalles == null ||
                    modelo.Detalles.Count == 0)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = "Faltan campos obligatorios o formato incorrecto"
                    });
                }

                // Validar sucursal
                var sucursal = await _context.Sucursales.FindAsync(modelo.Id_Sucursal);
                if (sucursal == null)
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "Sucursal no encontrada"
                    });
                }

                // Validar cliente
                var cliente = await _context.Clientes.FindAsync(modelo.Id_Cliente);
                if (cliente == null)
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "Cliente no encontrado"
                    });
                }

                // Validar duplicado
                bool existe = await _context.ReportesVentas
                    .AnyAsync(r =>
                        r.Id_Sucursal == modelo.Id_Sucursal &&
                        r.Año == modelo.Año &&
                        r.Mes == modelo.Mes
                    );

                if (existe)
                {
                    return Conflict(new
                    {
                        status = "error",
                        message = "Ya existe un reporte para la misma semana/año"
                    });
                }

                // Guardar CABECERA
                var reporte = new ReportesVentas
                {
                    Id_Sucursal = modelo.Id_Sucursal,
                    Año = modelo.Año,
                    Mes = modelo.Mes,
                    Total_Ventas = modelo.Total_Ventas,
                    Fecha_Recepcion = modelo.Fecha_Recepcion,
                    Id_Cliente = modelo.Id_Cliente
                };

                _context.ReportesVentas.Add(reporte);
                await _context.SaveChangesAsync();

                // Guardar DETALLES
                foreach (var d in modelo.Detalles)
                {
                    d.Id_Reporte = reporte.Id_Reporte;
                    _context.ReporteSemanalDetalles.Add(d);
                }

                await _context.SaveChangesAsync();

                return Created("", new
                {
                    status = "success",
                    message = "Reporte semanal registrado correctamente",
                    data = new
                    {
                        id_reporte = reporte.Id_Reporte,
                        id_sucursal = reporte.Id_Sucursal,
                        año = reporte.Año,
                        mes = reporte.Mes,
                        total_ventas = reporte.Total_Ventas
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error interno del servidor",
                    detalle = ex.Message
                });
            }
        }
    }
}