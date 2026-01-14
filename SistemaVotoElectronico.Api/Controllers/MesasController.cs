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
                var data = await _context.Mesas
                                         .Include(m => m.Eleccion)
                                         .ToListAsync();

                return ApiResult<List<Mesa>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Mesa>>.Fail(ex.Message);
            }
        }

        // GET: api/Mesas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Mesa>>> GetMesa(int id)
        {
            try
            {
                var mesa = await _context.Mesas
                                         .Include(m => m.Eleccion)
                                         .Include(m => m.JefeDeMesa) 
                                         .FirstOrDefaultAsync(m => m.Id == id);

                if (mesa == null) return ApiResult<Mesa>.Fail("Mesa no encontrada");

                return ApiResult<Mesa>.Ok(mesa);
            }
            catch (Exception ex)
            {
                return ApiResult<Mesa>.Fail(ex.Message);
            }
        }

        // PUT: api/Mesas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Mesa>>> PutMesa(int id, Mesa mesa)
        {
            if (id != mesa.Id) return ApiResult<Mesa>.Fail("IDs no coinciden");

            try
            {
                var mesaAnterior = await _context.Mesas.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                if (mesaAnterior == null) return ApiResult<Mesa>.Fail("La mesa no existe");

                _context.Entry(mesa).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return ApiResult<Mesa>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Mesa>.Fail(ex.Message);
            }
        }

        // POST: api/Mesas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<Mesa>>> PostMesa(Mesa mesa)
        {
            try
            {
                // Validar que la elección exista
                var eleccionExiste = await _context.Elecciones.AnyAsync(e => e.Id == mesa.EleccionId);
                if (!eleccionExiste)
                    return ApiResult<Mesa>.Fail("La elección especificada no existe.");

                _context.Mesas.Add(mesa);
                await _context.SaveChangesAsync();

                return ApiResult<Mesa>.Ok(mesa);
            }
            catch (Exception ex)
            {
                return ApiResult<Mesa>.Fail(ex.Message);
            }
        }

        // DELETE: api/Mesas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteMesa(int id)
        {
            try
            {
                var mesa = await _context.Mesas.FindAsync(id);
                if (mesa == null) return ApiResult<bool>.Fail("Mesa no encontrada");
                var tieneVotantes = await _context.PadronElectorales.AnyAsync(p => p.MesaId == id);
                if (tieneVotantes)
                    return ApiResult<bool>.Fail("No se puede borrar la mesa porque tiene votantes asignados en el padrón.");

                _context.Mesas.Remove(mesa);
                await _context.SaveChangesAsync();

                return ApiResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }
    }
}
