using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SistemaVotoElectronico.Api.Data;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MesasController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public MesasController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/Mesas
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Mesa>>>> GetMesas()
        {
            try
            {
                var data = await _context.Mesas.ToListAsync();
                Log.Information($"{data}");
                return ApiResult<List<Mesa>>.Ok(data);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<List<Mesa>>.Fail(ex.Message);
            }
        }

        // GET: api/Mesas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Mesa>>> GetMesa(int id)
        {
            try
            {
                var mesa = await _context
                    .Mesas
                    .Include(e => e.Eleccion)
                    .Include(e => e.JefeDeMesa)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (mesa == null)
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<Mesa>.Fail("Datos no encontrados");
                }
                Log.Information($"{mesa}");
                return ApiResult<Mesa>.Ok(mesa);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Mesa>.Fail(ex.Message);
            }
        }

        // PUT: api/Mesas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Mesa>>> PutMesa(int id, Mesa mesa)
        {
            if (id != mesa.Id)
            {
                Log.Information("Identificadores no coinciden");
                return ApiResult<Mesa>.Fail("Los identificadores no coinciden");
            }

            _context.Entry(mesa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!MesaExists(id))
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<Mesa>.Fail("Datos no encontrados");
                }
                else
                {
                    Log.Information(ex.Message);
                    return ApiResult<Mesa>.Fail(ex.Message);
                }
            }
            Log.Information($"{null}");
            return ApiResult<Mesa>.Ok(null);
        }

        // POST: api/Mesas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<Mesa>>> PostMesa(Mesa mesa)
        {
            try
            {
                _context.Mesas.Add(mesa);
                await _context.SaveChangesAsync();
                Log.Information($"{mesa}");
                return ApiResult<Mesa>.Ok(mesa);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Mesa>.Fail(ex.Message);
            }
        }

        // DELETE: api/Mesas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Mesa>>> DeleteMesa(int id)
        {
            try
            {
                var mesa = await _context.Mesas.FindAsync(id);
                if (mesa == null)
                {

                    Log.Information("Datos no encontrados");
                    return ApiResult<Mesa>.Fail("Datos no encontrados");
                }

                _context.Mesas.Remove(mesa);
                await _context.SaveChangesAsync();

                Log.Information($"{null}");
                return ApiResult<Mesa>.Ok(null);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Mesa>.Fail(ex.Message);
            }
        }

        private bool MesaExists(int id)
        {
            return _context.Mesas.Any(e => e.Id == id);
        }
    }
}
