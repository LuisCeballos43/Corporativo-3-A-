using Corporativo.Data;
using Corporativo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Corporativo.Controllers
{
    [Route("/corporativo/api/regiones")]
    public class RegionesController : Controller
    {
        private readonly CorporativoContext _context;

        public RegionesController(CorporativoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Region>>> GetRegiones()
        {
            var regiones = await _context.Regiones.ToListAsync();
            return Ok(regiones);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Region>> GetRegion(int id)
        {
            var region = await _context.Regiones.FindAsync(id);
            if (region == null)
            {
                return NotFound();
            }
            return Ok(region);
        }

        [HttpPost]
        public async Task<ActionResult<Region>> CreateRegion([FromBody] Region region)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Regiones.Add(region);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRegion), new { id = region.Id }, region);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRegion(int id, [FromBody] Region region)
        {
            if (id != region.Id)
            {
                return BadRequest("ID mismatch");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(region).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Regiones.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegion(int id)
        {
            var region = await _context.Regiones.FindAsync(id);
            if (region == null)
            {
                return NotFound();
            }
            // modify to set Activa to false instead of deleting
            region.Activa = false;
            _context.Entry(region).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
