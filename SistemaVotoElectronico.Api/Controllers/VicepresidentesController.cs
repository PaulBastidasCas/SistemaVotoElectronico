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
    public class VicepresidentesController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public VicepresidentesController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Vicepresidente>>>> GetVicepresidentes()
        {
            try
            {
                var data = await _context.Vicepresidentes.ToListAsync();
                return ApiResult<List<Vicepresidente>>.Ok(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener vicepresidentes");
                return ApiResult<List<Vicepresidente>>.Fail(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Vicepresidente>>> GetVicepresidente(int id)
        {
            try
            {
                var vicepresidente = await _context.Vicepresidentes.FindAsync(id);

                if (vicepresidente == null)
                {
                    return ApiResult<Vicepresidente>.Fail("Vicepresidente no encontrado");
                }

                return ApiResult<Vicepresidente>.Ok(vicepresidente);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener vicepresidente");
                return ApiResult<Vicepresidente>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Vicepresidente>>> PutVicepresidente(int id, Vicepresidente vicepresidente)
        {
            if (id != vicepresidente.Id)
            {
                return ApiResult<Vicepresidente>.Fail("Los IDs no coinciden");
            }

            _context.Entry(vicepresidente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ApiResult<Vicepresidente>.Ok(vicepresidente);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!VicepresidenteExists(id))
                {
                    return ApiResult<Vicepresidente>.Fail("Vicepresidente no encontrado");
                }
                else
                {
                    Log.Error(ex, "Error de concurrencia al actualizar vicepresidente");
                    return ApiResult<Vicepresidente>.Fail(ex.Message);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al actualizar vicepresidente");
                return ApiResult<Vicepresidente>.Fail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Vicepresidente>>> PostVicepresidente(Vicepresidente vicepresidente)
        {
            try
            {
                _context.Vicepresidentes.Add(vicepresidente);
                await _context.SaveChangesAsync();
                return ApiResult<Vicepresidente>.Ok(vicepresidente);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al crear vicepresidente");
                return ApiResult<Vicepresidente>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Vicepresidente>>> DeleteVicepresidente(int id)
        {
            try
            {
                var vicepresidente = await _context.Vicepresidentes.FindAsync(id);
                if (vicepresidente == null)
                {
                    return ApiResult<Vicepresidente>.Fail("Vicepresidente no encontrado");
                }

                _context.Vicepresidentes.Remove(vicepresidente);
                await _context.SaveChangesAsync();
                return ApiResult<Vicepresidente>.Ok(null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar vicepresidente");
                return ApiResult<Vicepresidente>.Fail(ex.Message);
            }
        }

        private bool VicepresidenteExists(int id)
        {
            return _context.Vicepresidentes.Any(e => e.Id == id);
        }
    }
}
