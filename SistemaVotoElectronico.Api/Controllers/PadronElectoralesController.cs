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
    public class PadronElectoralesController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public PadronElectoralesController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/PadronElectorales
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<PadronElectoral>>>> GetPadronElectoral()
        {
            try
            {
                var data = await _context.PadronElectorales.ToListAsync();
                return ApiResult<List<PadronElectoral>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<PadronElectoral>>.Fail(ex.Message);
            }
        }

        // GET: api/PadronElectorales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<PadronElectoral>>> GetPadronElectoral(int id)
        {
            try
            {
                var padronElectoral = await _context
                    .PadronElectorales
                    .Include(e => e.Eleccion)
                    .FirstOrDefaultAsync( e  => e.Id == id);

                if (padronElectoral == null)
                {
                    return ApiResult<PadronElectoral>.Fail("Datos no encontrados");
                }

                return ApiResult<PadronElectoral>.Ok(padronElectoral);
            }
            catch (Exception ex)
            {
                return ApiResult<PadronElectoral>.Fail(ex.Message);
            }
        }

        // PUT: api/PadronElectorales/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<PadronElectoral>>> PutPadronElectoral(int id, PadronElectoral padronElectoral)
        {
            if (id != padronElectoral.Id)
            {
                return ApiResult<PadronElectoral>.Fail("Los identificadores no coinciden");
            }

            _context.Entry(padronElectoral).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PadronElectoralExists(id))
                {
                    return ApiResult<PadronElectoral>.Fail("Datos no encontrados");
                }
                else
                {
                    return ApiResult<PadronElectoral>.Fail(ex.Message);
                }
            }

            return ApiResult<PadronElectoral>.Ok(null);
        }

        // POST: api/PadronElectorales
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<PadronElectoral>>> PostPadronElectoral(PadronElectoral padronElectoral)
        {
            try
            {
                _context.PadronElectorales.Add(padronElectoral);
                await _context.SaveChangesAsync();

                return ApiResult<PadronElectoral>.Ok(padronElectoral);
            }
            catch (Exception ex)
            {
                return ApiResult<PadronElectoral>.Fail(ex.Message);
            }
        }

        // DELETE: api/PadronElectorales/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<PadronElectoral>>> DeletePadronElectoral(int id)
        {
            try
            {
                var padronElectoral = await _context.PadronElectorales.FindAsync(id);
                if (padronElectoral == null)
                {
                    return ApiResult<PadronElectoral>.Fail("Datos no encontrados");
                }

                _context.PadronElectorales.Remove(padronElectoral);
                await _context.SaveChangesAsync();

                return ApiResult<PadronElectoral>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<PadronElectoral>.Fail(ex.Message);
            }
        }

        private bool PadronElectoralExists(int id)
        {
            return _context.PadronElectorales.Any(e => e.Id == id);
        }
    }
}
