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
    public class EleccionesController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public EleccionesController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/Elecciones
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Eleccion>>>> GetEleccion()
        {
            try
            {
                var data = await _context.Elecciones.ToListAsync();
                Log.Information($"{data}");
                return ApiResult<List<Eleccion>>.Ok(data);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<List<Eleccion>>.Fail(ex.Message);
            }
        }

        // GET: api/Elecciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Eleccion>>> GetEleccion(int id)
        {
            try
            {
                var eleccion = await _context
                    .Elecciones
                    .Include(e  => e.Listas)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (eleccion == null)
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<Eleccion>.Fail("Datos no encontrados");
                }
                Log.Information($"{eleccion}");
                return ApiResult<Eleccion>.Ok(eleccion);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Eleccion>.Fail(ex.Message);
            }
        }

        // PUT: api/Elecciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Eleccion>>> PutEleccion(int id, Eleccion eleccion)
        {
            if (id != eleccion.Id)
            {
                Log.Information("Identificadores no coinciden");
                return ApiResult<Eleccion>.Fail("Identificadores no coinciden");
            }

            _context.Entry(eleccion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!EleccionExists(id))
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<Eleccion>.Fail("Datos no encontrados");
                }
                else
                {
                    Log.Information(ex.Message);
                    return ApiResult<Eleccion>.Fail(ex.Message);
                }
            }
            Log.Information($"{null}");
            return ApiResult<Eleccion>.Ok(null);
        }

        // POST: api/Elecciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<Eleccion>>> PostEleccion(Eleccion eleccion)
        {
            try
            {
                _context.Elecciones.Add(eleccion);
                await _context.SaveChangesAsync();
                Log.Information($"{eleccion}");
                return ApiResult<Eleccion>.Ok(eleccion);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Eleccion>.Fail(ex.Message);
            }
           
        }

        // DELETE: api/Elecciones/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Eleccion>>> DeleteEleccion(int id)
        {
            try
            {
                var eleccion = await _context.Elecciones.FindAsync(id);
                if (eleccion == null)
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<Eleccion>.Fail("Datos no encontrados");
                }

                _context.Elecciones.Remove(eleccion);
                await _context.SaveChangesAsync();
                Log.Information($"{null}");
                return ApiResult<Eleccion>.Ok(null);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Eleccion>.Fail(ex.Message);
            }
            
        }

        private bool EleccionExists(int id)
        {
            return _context.Elecciones.Any(e => e.Id == id);
        }
    }
}
