using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SistemaVotoElectronico.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                var data = await _context.JefesDeMesa.ToListAsync();
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

            _context.Entry(jefeDeMesa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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
            Log.Information($"{null}");
            return ApiResult<JefeDeMesa>.Ok(null);
        }

        // POST: api/JefesDeMesa
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<JefeDeMesa>>> PostJefeDeMesa(JefeDeMesa jefeDeMesa)
        {
            try
            {
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
