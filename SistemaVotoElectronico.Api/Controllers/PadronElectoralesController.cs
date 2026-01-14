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
                Log.Information($"{data}");
                return ApiResult<List<PadronElectoral>>.Ok(data);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
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
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (padronElectoral == null)
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<PadronElectoral>.Fail("Datos no encontrados");
                }

                return ApiResult<PadronElectoral>.Ok(padronElectoral);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
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
                Log.Information("Los identificadores no coinciden");
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
                    Log.Information("Datos no encontrados");
                    return ApiResult<PadronElectoral>.Fail("Datos no encontrados");
                }
                else
                {
                    Log.Information(ex.Message);
                    return ApiResult<PadronElectoral>.Fail(ex.Message);
                }
            }
            Log.Information($"{null}");
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
                Log.Information($"{padronElectoral}");
                return ApiResult<PadronElectoral>.Ok(padronElectoral);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
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
                    Log.Information("Datos no encontrados");
                    return ApiResult<PadronElectoral>.Fail("Datos no encontrados");
                }

                _context.PadronElectorales.Remove(padronElectoral);
                await _context.SaveChangesAsync();
                Log.Information($"{null}");
                return ApiResult<PadronElectoral>.Ok(null);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<PadronElectoral>.Fail(ex.Message);
            }
        }

        private bool PadronElectoralExists(int id)
        {
            return _context.PadronElectorales.Any(e => e.Id == id);
        }

        [HttpPost("generar-codigo")]
        public async Task<ActionResult<ApiResult<object>>> GenerarCodigo([FromBody] GenerarCodigoRequest request)
        {
            try
            {
                var registroPadron = await _context.PadronElectorales
                    .Include(p => p.Votante)
                    .FirstOrDefaultAsync(p => p.Votante.Cedula == request.CedulaVotante
                                           && p.EleccionId == request.EleccionId);

                if (registroPadron == null)
                    return ApiResult<object>.Fail("El votante no está empadronado en esta elección.");

                if (registroPadron.MesaId != request.MesaId)
                    return ApiResult<object>.Fail("Este votante pertenece a otra mesa.");

                if (registroPadron.CodigoCanjeado)
                    return ApiResult<object>.Fail("ALERTA: Este votante ya ejerció su voto.");

                if (!string.IsNullOrEmpty(registroPadron.CodigoEnlace))
                {
                    return ApiResult<object>.Ok(new
                    {
                        Mensaje = "Código recuperado (ya existía)",
                        Codigo = registroPadron.CodigoEnlace,
                        Nombre = registroPadron.Votante.NombreCompleto
                    });
                }

                string codigoNuevo = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

                registroPadron.CodigoEnlace = codigoNuevo;
                registroPadron.FechaGeneracionCodigo = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResult<object>.Ok(new
                {
                    Mensaje = "Código generado exitosamente",
                    Codigo = codigoNuevo,
                    Nombre = registroPadron.Votante.NombreCompleto
                });
            }
            catch (Exception ex)
            {
                return ApiResult<object>.Fail(ex.Message);
            }
        }
    }

    public class GenerarCodigoRequest
    {
        public string CedulaVotante { get; set; }
        public int EleccionId { get; set; }
        public int MesaId { get; set; }
    }
}
