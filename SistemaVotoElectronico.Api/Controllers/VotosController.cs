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
    public class VotosController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public VotosController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/Votos
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Voto>>>> GetVoto()
        {
            try
            {
                var data = await _context.Votos.ToListAsync();
                return ApiResult<List<Voto>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Voto>>.Fail(ex.Message);
            }
        }

        // GET: api/Votos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Voto>>> GetVoto(Guid id)
        {
            try
            {
                var voto = await _context
                    .Votos
                    .Include(e => e.Eleccion)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (voto == null)
                {
                    return ApiResult<Voto>.Fail("Datos no encontrados");
                }

                return ApiResult<Voto>.Ok(voto);
            }
            catch (Exception ex)
            {
                return ApiResult<Voto>.Fail(ex.Message);
            }
        }

        // PUT: api/Votos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Voto>>> PutVoto(Guid id, Voto voto)
        {
            if (id != voto.Id)
            {
                return ApiResult<Voto>.Fail("Los identificadores no coinciden");
            }

            _context.Entry(voto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!VotoExists(id))
                {
                    return ApiResult<Voto>.Fail("Datos no encontrados");
                }
                else
                {
                    return ApiResult<Voto>.Fail(ex.Message);
                }
            }

            return ApiResult<Voto>.Ok(null);
        }

        // POST: api/Votos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<Voto>>> PostVoto(Voto voto)
        {
            try
            {
                _context.Votos.Add(voto);
                await _context.SaveChangesAsync();

                return ApiResult<Voto>.Ok(voto);
            }
            catch (Exception ex)
            {
                return ApiResult<Voto>.Fail(ex.Message);
            }
        }

        // DELETE: api/Votos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Voto>>> DeleteVoto(Guid id)
        {
            try
            {

                var voto = await _context.Votos.FindAsync(id);
                if (voto == null)
                {
                    return ApiResult<Voto>.Fail("Datos no encontrados");
                }

                _context.Votos.Remove(voto);
                await _context.SaveChangesAsync();

                return ApiResult<Voto>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Voto>.Fail(ex.Message);
            }
        }

        private bool VotoExists(Guid id)
        {
            return _context.Votos.Any(e => e.Id == id);
        }
    }
}
