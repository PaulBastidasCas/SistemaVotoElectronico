using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PadronElectoralesController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public PadronElectoralesController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/PadronElectorales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PadronElectoral>>> GetPadronElectoral()
        {
            return await _context.PadronElectoral.ToListAsync();
        }

        // GET: api/PadronElectorales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PadronElectoral>> GetPadronElectoral(int id)
        {
            var padronElectoral = await _context.PadronElectoral.FindAsync(id);

            if (padronElectoral == null)
            {
                return NotFound();
            }

            return padronElectoral;
        }

        // PUT: api/PadronElectorales/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPadronElectoral(int id, PadronElectoral padronElectoral)
        {
            if (id != padronElectoral.Id)
            {
                return BadRequest();
            }

            _context.Entry(padronElectoral).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PadronElectoralExists(id))
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

        // POST: api/PadronElectorales
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PadronElectoral>> PostPadronElectoral(PadronElectoral padronElectoral)
        {
            _context.PadronElectoral.Add(padronElectoral);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPadronElectoral", new { id = padronElectoral.Id }, padronElectoral);
        }

        // DELETE: api/PadronElectorales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePadronElectoral(int id)
        {
            var padronElectoral = await _context.PadronElectoral.FindAsync(id);
            if (padronElectoral == null)
            {
                return NotFound();
            }

            _context.PadronElectoral.Remove(padronElectoral);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PadronElectoralExists(int id)
        {
            return _context.PadronElectoral.Any(e => e.Id == id);
        }
    }
}
