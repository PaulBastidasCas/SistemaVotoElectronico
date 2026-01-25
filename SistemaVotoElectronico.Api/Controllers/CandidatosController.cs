using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SistemaVotoElectronico.Api.Data;
using SistemaVotoElectronico.Modelos.Entidades;
using SistemaVotoElectronico.Modelos.Responses;

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
                var data = await _context.Candidatos.AsNoTracking().ToListAsync();
                Log.Information($"{data}");
                return ApiResult<List<Candidato>>.Ok(data);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
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
                    .AsNoTracking()
                    .Include(e => e.ListaElectoral)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (candidato == null) 
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<Candidato>.Fail("Datos no encontrados");
                }
                Log.Information($"{candidato}");
                return ApiResult<Candidato>.Ok(candidato);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Candidato>.Fail(ex.Message);
            }
        }

        // PUT: api/Candidatos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Candidato>>> PutCandidato(int id, Candidato candidato)
        {
            if (id != candidato.Id) return ApiResult<Candidato>.Fail("ID no coincide");

            try
            {
                var original = await _context.Candidatos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (original == null) return ApiResult<Candidato>.Fail("No encontrado");

                if (string.IsNullOrEmpty(candidato.Contrasena))
                {
                    candidato.Contrasena = original.Contrasena;
                }
                else
                {
                    candidato.Contrasena = BCrypt.Net.BCrypt.HashPassword(candidato.Contrasena);
                }

                _context.Entry(candidato).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return ApiResult<Candidato>.Ok(null);
            }
            catch (Exception ex) { return ApiResult<Candidato>.Fail(ex.Message); }
        }

        // POST: api/Candidatos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<Candidato>>> PostCandidato(Candidato candidato)
        {
            try
            {
                if (!string.IsNullOrEmpty(candidato.Contrasena))
                {
                    candidato.Contrasena = BCrypt.Net.BCrypt.HashPassword(candidato.Contrasena);
                }

                _context.Candidatos.Add(candidato);
                await _context.SaveChangesAsync();

                Log.Information($"{candidato}");
                return ApiResult<Candidato>.Ok(candidato);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
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
                    Log.Information("Datos no encontrados");
                    return ApiResult<Candidato>.Fail("Datos no encontrados");
                }

                _context.Candidatos.Remove(candidato);
                await _context.SaveChangesAsync();
                Log.Information($"{null}");
                return ApiResult<Candidato>.Ok(null);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Candidato>.Fail(ex.Message);
            }
           
        }

        private bool CandidatoExists(int id)
        {
            return _context.Candidatos.Any(e => e.Id == id);
        }
    }
}
