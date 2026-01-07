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
    public class CandidatosController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public CandidatosController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/Candidatos
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Candidato>>>> GetCandidato()
        {
            try
            {
                var data = await _context.Candidatos.ToListAsync();
                return ApiResult<List<Candidato>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Candidato>>.Fail(ex.Message);
            }
        }

        // GET: api/Candidatos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Candidato>>> GetCandidato(int id)
        {
            try
            {
                var candidato = await _context
                    .Candidatos
                    .Include(e => e.ListaElectoral)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (candidato == null) 
                {
                    return ApiResult<Candidato>.Fail("Datos no encontrados");
                }

                return ApiResult<Candidato>.Ok(candidato);
            }
            catch (Exception ex)
            {
                return ApiResult<Candidato>.Fail(ex.Message);
            }
        }

        // PUT: api/Candidatos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Candidato>>> PutCandidato(int id, Candidato candidato)
        {
            if (id != candidato.Id)
            {
                return ApiResult<Candidato>.Fail("No coinciden los identificadores");
            }

            _context.Entry(candidato).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CandidatoExists(id))
                {
                    return ApiResult<Candidato>.Fail("Datos no encontrados");
                }
                else
                {
                    return ApiResult<Candidato>.Fail(ex.Message);
                }
            }

            return ApiResult<Candidato>.Ok(null);
        }

        // POST: api/Candidatos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<Candidato>>> PostCandidato(Candidato candidato)
        {
            try
            {
                _context.Candidatos.Add(candidato);
                await _context.SaveChangesAsync();

                return ApiResult<Candidato>.Ok(candidato);
            }
            catch (Exception ex)
            {
                return ApiResult<Candidato>.Fail(ex.Message);
            }
            
        }

        // DELETE: api/Candidatos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Candidato>>> DeleteCandidato(int id)
        {
            try
            {
                var candidato = await _context.Candidatos.FindAsync(id);
                if (candidato == null)
                {
                    return ApiResult<Candidato>.Fail("Datos no encontrados");
                }

                _context.Candidatos.Remove(candidato);
                await _context.SaveChangesAsync();

                return ApiResult<Candidato>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Candidato>.Fail(ex.Message);
            }
           
        }

        private bool CandidatoExists(int id)
        {
            return _context.Candidatos.Any(e => e.Id == id);
        }
    }
}
