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
    public class VotantesController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public VotantesController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/Votantes
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Votante>>>> GetVotante()
        {
            try
            {
                var data = await _context.Votantes.ToListAsync();
                return ApiResult<List<Votante>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Votante>>.Fail(ex.Message);
            }
        }

        // GET: api/Votantes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Votante>>> GetVotante(int id)
        {
            try
            {
                var votante = await _context
                    .Votantes
                    .Include(e => e.HistorialVotos)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (votante == null)
                {
                    return ApiResult<Votante>.Fail("Datos no encontrados");
                }

                return ApiResult<Votante>.Ok(votante);
            }
            catch (Exception ex)
            {
                return ApiResult<Votante>.Fail(ex.Message);
            }
        }

        // PUT: api/Votantes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Votante>>> PutVotante(int id, Votante votante)
        {
            if (id != votante.Id)
            {
                return ApiResult<Votante>.Fail("Los identificadores no coinciden");
            }

            _context.Entry(votante).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!VotanteExists(id))
                {
                    return ApiResult<Votante>.Fail("Datos no encontrados");
                }
                else
                {
                    return ApiResult<Votante>.Fail(ex.Message);
                }
            }

            return ApiResult<Votante>.Ok(null);
        }

        // POST: api/Votantes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<Votante>>> PostVotante(Votante votante)
        {
            try
            {
                _context.Votantes.Add(votante);
                await _context.SaveChangesAsync();

                return ApiResult<Votante>.Ok(votante);
            }
            catch (Exception ex)
            {
                return ApiResult<Votante>.Fail(ex.Message);
            }
        }

        // DELETE: api/Votantes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Votante>>> DeleteVotante(int id)
        {
            try
            {
                var votante = await _context.Votantes.FindAsync(id);
                if (votante == null)
                {
                    return ApiResult<Votante>.Fail("Datos no encontrados");
                }

                _context.Votantes.Remove(votante);
                await _context.SaveChangesAsync();

                return ApiResult<Votante>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Votante>.Fail(ex.Message);
            }
        }

        private bool VotanteExists(int id)
        {
            return _context.Votantes.Any(e => e.Id == id);
        }
    }
}
