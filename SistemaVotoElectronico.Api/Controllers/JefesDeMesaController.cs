using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SistemaVotoElectronico.Api.Data;
using SistemaVotoElectronico.Modelos.Entidades;
using SistemaVotoElectronico.Modelos.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                // 1. Extraer jefes de mesa incluyendo datos de la mesa asignada (AsNoTracking para rendimiento de lectura)
                var data = await _context
                    .JefesDeMesa
                    .AsNoTracking()
                    .Include(e => e.MesaAsignada)
                    .ToListAsync();
                Log.Information($"{data}");
                return ApiResult<List<JefeDeMesa>>.Ok(data);
            }
            catch (Exception ex)
            {
                // 2. Manejo de errores
                Log.Information(ex.Message);
                return ApiResult<List<JefeDeMesa>>.Fail(ex.Message);
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
                    .AsNoTracking()
                    .Include(e => e.MesaAsignada)
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
        public async Task<ActionResult<ApiResult<JefeDeMesa>>> PutJefeDeMesa(int id, JefeDeMesa jefe)
        {
            if (id != jefe.Id) return ApiResult<JefeDeMesa>.Fail("ID no coincide");

            try
            {
                var original = await _context.JefesDeMesa.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (original == null) return ApiResult<JefeDeMesa>.Fail("No encontrado");

                if (string.IsNullOrEmpty(jefe.Contrasena))
                {
                    jefe.Contrasena = original.Contrasena;
                }
                else
                {
                    jefe.Contrasena = BCrypt.Net.BCrypt.HashPassword(jefe.Contrasena);
                }

                _context.Entry(jefe).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return ApiResult<JefeDeMesa>.Ok(null);
            }
            catch (Exception ex) { return ApiResult<JefeDeMesa>.Fail(ex.Message); }
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
