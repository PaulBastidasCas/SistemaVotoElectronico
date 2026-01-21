using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotoElectronico.Api.Data;
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

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<PadronElectoral>>>> GetPadronElectorales()
        {
            try
            {
                var data = await _context.PadronElectorales
                    .Include(p => p.Votante)  
                    .Include(p => p.Mesa)     
                    .Include(p => p.Eleccion) 
                    .ToListAsync();

                return ApiResult<List<PadronElectoral>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<PadronElectoral>>.Fail(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<PadronElectoral>>> GetPadronElectoral(int id)
        {
            try
            {
                var padronElectoral = await _context.PadronElectorales
                    .Include(p => p.Votante)
                    .Include(p => p.Mesa)
                    .Include(p => p.Eleccion)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (padronElectoral == null)
                {
                    return ApiResult<PadronElectoral>.Fail("Registro no encontrado");
                }

                return ApiResult<PadronElectoral>.Ok(padronElectoral);
            }
            catch (Exception ex)
            {
                return ApiResult<PadronElectoral>.Fail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<PadronElectoral>>> PostPadronElectoral(PadronElectoral padronElectoral)
        {
            try
            {
                bool existe = await _context.PadronElectorales
                    .AnyAsync(p => p.VotanteId == padronElectoral.VotanteId &&
                                   p.EleccionId == padronElectoral.EleccionId);

                if (existe)
                {
                    return ApiResult<PadronElectoral>.Fail("Este votante ya está empadronado en esta elección.");
                }

                _context.PadronElectorales.Add(padronElectoral);
                await _context.SaveChangesAsync();

                return ApiResult<PadronElectoral>.Ok(padronElectoral);
            }
            catch (Exception ex)
            {
                return ApiResult<PadronElectoral>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<PadronElectoral>>> PutPadronElectoral(int id, PadronElectoral padronElectoral)
        {
            if (id != padronElectoral.Id)
            {
                return ApiResult<PadronElectoral>.Fail("Los IDs no coinciden");
            }

            _context.Entry(padronElectoral).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ApiResult<PadronElectoral>.Ok(padronElectoral);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PadronElectoralExists(id))
                {
                    return ApiResult<PadronElectoral>.Fail("Registro no encontrado al intentar actualizar");
                }
                else
                {
                    return ApiResult<PadronElectoral>.Fail(ex.Message);
                }
            }
            catch (Exception ex)
            {
                return ApiResult<PadronElectoral>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<PadronElectoral>>> DeletePadronElectoral(int id)
        {
            try
            {
                var padronElectoral = await _context.PadronElectorales.FindAsync(id);
                if (padronElectoral == null)
                {
                    return ApiResult<PadronElectoral>.Fail("Registro no encontrado");
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

        [HttpPost("generar-codigo")]
        public async Task<ActionResult<ApiResult<string>>> GenerarCodigo([FromBody] GenerarCodigoRequest request)
        {
            try
            {
                // 1. Validaciones básicas
                if (request == null || string.IsNullOrEmpty(request.CedulaVotante) || request.EleccionId == 0 || request.MesaId == 0)
                {
                    return ApiResult<string>.Fail("Datos incompletos.");
                }

                // 2. Buscar registro
                var registro = await _context.PadronElectorales
                    .Include(p => p.Votante)
                    .FirstOrDefaultAsync(p =>
                        p.Votante != null &&
                        p.Votante.Cedula == request.CedulaVotante &&
                        p.EleccionId == request.EleccionId);

                if (registro == null)
                    return ApiResult<string>.Fail("Ciudadano no empadronado en esta elección.");

                if (registro.MesaId != request.MesaId)
                    return ApiResult<string>.Fail($"Este votante pertenece a otra mesa (Mesa ID: {registro.MesaId}).");

                if (registro.CodigoCanjeado)
                    return ApiResult<string>.Fail("El votante ya ha sufragado.");

                // 3. Generar Código
                string caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
                Random random = new Random();
                string nuevoCodigo = new string(Enumerable.Repeat(caracteres, 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                registro.CodigoEnlace = nuevoCodigo;
                registro.FechaGeneracionCodigo = DateTime.Now;
                await _context.SaveChangesAsync();

                return ApiResult<string>.Ok(nuevoCodigo);
            }
            catch (Exception ex)
            {
                return ApiResult<string>.Fail("Error interno: " + ex.Message);
            }
        }

        private bool PadronElectoralExists(int id)
        {
            return _context.PadronElectorales.Any(e => e.Id == id);
        }
    }
}