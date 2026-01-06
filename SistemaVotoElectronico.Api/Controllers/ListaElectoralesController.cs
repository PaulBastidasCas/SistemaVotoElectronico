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
    public class ListaElectoralesController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public ListaElectoralesController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/ListaElectorales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListaElectoral>>> GetListaElectoral()
        {
            return await _context.ListaElectorales.ToListAsync();
        }

        // GET: api/ListaElectorales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ListaElectoral>> GetListaElectoral(int id)
        {
            var listaElectoral = await _context.ListaElectorales.FindAsync(id);

            if (listaElectoral == null)
            {
                return NotFound();
            }

            return listaElectoral;
        }

        // PUT: api/ListaElectorales/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutListaElectoral(int id, ListaElectoral listaElectoral)
        {
            if (id != listaElectoral.Id)
            {
                return BadRequest();
            }

            _context.Entry(listaElectoral).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListaElectoralExists(id))
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

        // POST: api/ListaElectorales
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ListaElectoral>> PostListaElectoral(ListaElectoral listaElectoral)
        {
            _context.ListaElectorales.Add(listaElectoral);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetListaElectoral", new { id = listaElectoral.Id }, listaElectoral);
        }

        // DELETE: api/ListaElectorales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListaElectoral(int id)
        {
            var listaElectoral = await _context.ListaElectorales.FindAsync(id);
            if (listaElectoral == null)
            {
                return NotFound();
            }

            _context.ListaElectorales.Remove(listaElectoral);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ListaElectoralExists(int id)
        {
            return _context.ListaElectorales.Any(e => e.Id == id);
        }
    }
}
