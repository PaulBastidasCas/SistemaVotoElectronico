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
    public class PresidentesController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public PresidentesController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Presidente>>>> GetPresidentes()
        {
            try
            {
                var data = await _context.Presidentes.ToListAsync();
                return ApiResult<List<Presidente>>.Ok(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener presidentes");
                return ApiResult<List<Presidente>>.Fail(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Presidente>>> GetPresidente(int id)
        {
            try
            {
                var presidente = await _context.Presidentes.FindAsync(id);

                if (presidente == null)
                {
                    return ApiResult<Presidente>.Fail("Presidente no encontrado");
                }

                return ApiResult<Presidente>.Ok(presidente);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener presidente");
                return ApiResult<Presidente>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Presidente>>> PutPresidente(int id, Presidente presidente)
        {
            if (id != presidente.Id)
            {
                return ApiResult<Presidente>.Fail("Los IDs no coinciden");
            }

            _context.Entry(presidente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ApiResult<Presidente>.Ok(presidente);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PresidenteExists(id))
                {
                    return ApiResult<Presidente>.Fail("Presidente no encontrado");
                }
                else
                {
                    Log.Error(ex, "Error de concurrencia al actualizar presidente");
                    return ApiResult<Presidente>.Fail(ex.Message);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al actualizar presidente");
                return ApiResult<Presidente>.Fail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Presidente>>> PostPresidente(Presidente presidente)
        {
            try
            {
                _context.Presidentes.Add(presidente);
                await _context.SaveChangesAsync();
                return ApiResult<Presidente>.Ok(presidente);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al crear presidente");
                return ApiResult<Presidente>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Presidente>>> DeletePresidente(int id)
        {
            try
            {
                var presidente = await _context.Presidentes.FindAsync(id);
                if (presidente == null)
                {
                    return ApiResult<Presidente>.Fail("Presidente no encontrado");
                }

                _context.Presidentes.Remove(presidente);
                await _context.SaveChangesAsync();
                return ApiResult<Presidente>.Ok(null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar presidente");
                return ApiResult<Presidente>.Fail(ex.Message);
            }
        }

        private bool PresidenteExists(int id)
        {
            return _context.Presidentes.Any(e => e.Id == id);
        }
    }
}
