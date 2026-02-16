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
    public class AsambleistasController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public AsambleistasController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Asambleista>>>> GetAsambleistas()
        {
            try
            {
                var data = await _context.Asambleistas.ToListAsync();
                return ApiResult<List<Asambleista>>.Ok(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener asambleistas");
                return ApiResult<List<Asambleista>>.Fail(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Asambleista>>> GetAsambleista(int id)
        {
            try
            {
                var asambleista = await _context.Asambleistas.FindAsync(id);

                if (asambleista == null)
                {
                    return ApiResult<Asambleista>.Fail("Asambleista no encontrado");
                }

                return ApiResult<Asambleista>.Ok(asambleista);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener asambleista");
                return ApiResult<Asambleista>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Asambleista>>> PutAsambleista(int id, Asambleista asambleista)
        {
            if (id != asambleista.Id)
            {
                return ApiResult<Asambleista>.Fail("Los IDs no coinciden");
            }

            _context.Entry(asambleista).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ApiResult<Asambleista>.Ok(asambleista);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!AsambleistaExists(id))
                {
                    return ApiResult<Asambleista>.Fail("Asambleista no encontrado");
                }
                else
                {
                    Log.Error(ex, "Error de concurrencia al actualizar asambleista");
                    return ApiResult<Asambleista>.Fail(ex.Message);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al actualizar asambleista");
                return ApiResult<Asambleista>.Fail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Asambleista>>> PostAsambleista(Asambleista asambleista)
        {
            try
            {
                _context.Asambleistas.Add(asambleista);
                await _context.SaveChangesAsync();
                return ApiResult<Asambleista>.Ok(asambleista);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al crear asambleista");
                return ApiResult<Asambleista>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Asambleista>>> DeleteAsambleista(int id)
        {
            try
            {
                var asambleista = await _context.Asambleistas.FindAsync(id);
                if (asambleista == null)
                {
                    return ApiResult<Asambleista>.Fail("Asambleista no encontrado");
                }

                _context.Asambleistas.Remove(asambleista);
                await _context.SaveChangesAsync();
                return ApiResult<Asambleista>.Ok(null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar asambleista");
                return ApiResult<Asambleista>.Fail(ex.Message);
            }
        }

        private bool AsambleistaExists(int id)
        {
            return _context.Asambleistas.Any(e => e.Id == id);
        }
    }
}
