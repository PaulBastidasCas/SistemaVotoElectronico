using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SistemaVotoElectronico.Api.Data;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JefesDeMesaController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public JefesDeMesaController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/JefesDeMesa
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<JefeDeMesa>>>> GetJefesDeMesa()
        {
            try
            {
                var data = await _context
                    .JefesDeMesa
                    .Include(e => e.MesaAsignada) 
                    .ToListAsync();
                Log.Information($"{data}");
                return ApiResult<List<JefeDeMesa>>.Ok( data );
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<List<JefeDeMesa>>.Fail(ex.Message );
            }
        }

        // GET: api/JefesDeMesa/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<JefeDeMesa>>> GetJefeDeMesa(int id)
        {
            try
            {
                var jefeDeMesa = await _context
                    .JefesDeMesa
                    .Include(e  => e.MesaAsignada)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (jefeDeMesa == null)
                {

                    Log.Information("Datos no encontrados");
                    return ApiResult<JefeDeMesa>.Fail("Datos no encontrados");
                }
                Log.Information($"{jefeDeMesa}");
                return ApiResult<JefeDeMesa>.Ok(jefeDeMesa);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<JefeDeMesa>.Fail(ex.Message);
            }
        }

        // PUT: api/JefesDeMesa/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<JefeDeMesa>>> PutJefeDeMesa(int id, JefeDeMesa jefeDeMesa)
        {
            if (id != jefeDeMesa.Id)
            {
                Log.Information("Identificadores no coinciden");
                return ApiResult<JefeDeMesa>.Fail("Los identificadores no coinciden");
            }

            try
            {
                var jefeAnterior = await _context.JefesDeMesa
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(x => x.Id == id);

                if (jefeAnterior == null) return ApiResult<JefeDeMesa>.Fail("No existe");

                if (string.IsNullOrEmpty(jefeDeMesa.Contrasena))
                {
                    jefeDeMesa.Contrasena = jefeAnterior.Contrasena;
                }
                else
                {
                    jefeDeMesa.Contrasena = BCrypt.Net.BCrypt.HashPassword(jefeDeMesa.Contrasena);
                }

                _context.Entry(jefeDeMesa).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return ApiResult<JefeDeMesa>.Ok(null);
            }
            catch (DbUpdateConcurrencyException ex) 
            {
                if (!JefeDeMesaExists(id))
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<JefeDeMesa>.Fail("Datos no encontrados");
                }
                else
                {
                    Log.Information(ex.Message);
                    return ApiResult<JefeDeMesa>.Fail(ex.Message);
                }
            }
        }

        // POST: api/JefesDeMesa
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<JefeDeMesa>>> PostJefeDeMesa(JefeDeMesa jefeDeMesa)
        {
            try
            {
                if (string.IsNullOrEmpty(jefeDeMesa.Contrasena))
                    return ApiResult<JefeDeMesa>.Fail("La contraseña es obligatoria");

                jefeDeMesa.Contrasena = BCrypt.Net.BCrypt.HashPassword(jefeDeMesa.Contrasena);

                _context.JefesDeMesa.Add(jefeDeMesa);
                await _context.SaveChangesAsync();

                Log.Information($"{jefeDeMesa}");
                return ApiResult<JefeDeMesa>.Ok(jefeDeMesa);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<JefeDeMesa>.Fail(ex.Message);
            }
        }

        // DELETE: api/JefesDeMesa/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<JefeDeMesa>>> DeleteJefeDeMesa(int id)
        {
            try
            {
                var jefeDeMesa = await _context.JefesDeMesa.FindAsync(id);
                if (jefeDeMesa == null)
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<JefeDeMesa>.Fail("Datos no encontrados");
                }

                _context.JefesDeMesa.Remove(jefeDeMesa);
                await _context.SaveChangesAsync();
                Log.Information($"{null}");
                return ApiResult<JefeDeMesa>.Ok(null);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<JefeDeMesa>.Fail(ex.Message);
            }
        }

        private bool JefeDeMesaExists(int id)
        {
            return _context.JefesDeMesa.Any(e => e.Id == id);
        }
    }
}
