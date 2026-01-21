using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SistemaVotoElectronico.Api.Data;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotosController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public VotosController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        [HttpPost("emitir")]
        public async Task<ActionResult<ApiResult<string>>> EmitirVoto([FromBody] VotoRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var registroPadron = await _context.PadronElectorales
                    .FirstOrDefaultAsync(p => p.CodigoEnlace == request.CodigoEnlace
                                           && p.EleccionId == request.EleccionId);

                if (registroPadron == null)
                {
                    Log.Warning($"Intento de voto fallido. Código no existe o elección incorrecta: {request.CodigoEnlace}");
                    return ApiResult<string>.Fail("El código de enlace no es válido para esta elección.");
                }

                if (registroPadron.CodigoCanjeado)
                {
                    Log.Warning($"Intento de doble voto. Código: {request.CodigoEnlace}");
                    return ApiResult<string>.Fail("Este código de activación ya fue utilizado previamente.");
                }

                var nuevoVoto = new Voto
                {
                    Id = Guid.NewGuid(), 
                    EleccionId = request.EleccionId,
                    IdListaSeleccionada = request.IdListaSeleccionada,
                    IdCandidatoSeleccionado = request.IdCandidatoSeleccionado,
                    FechaRegistro = DateTime.Now
                };

                _context.Votos.Add(nuevoVoto);

                registroPadron.CodigoCanjeado = true;
                registroPadron.FechaVoto = DateTime.Now;

                registroPadron.VotoPlanchaRealizado = (request.IdListaSeleccionada != null);
                registroPadron.VotoNominalRealizado = (request.IdCandidatoSeleccionado != null);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                Log.Information($"Voto registrado con éxito. Código quemado: {request.CodigoEnlace}");
                return ApiResult<string>.Ok("Su voto ha sido registrado correctamente.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error($"Error crítico al emitir voto: {ex.Message}");
                return ApiResult<string>.Fail("Ocurrió un error interno al procesar su voto. Intente nuevamente.");
            }
        }

        // GET: api/Votos
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Voto>>>> GetVotos()
        {
            try
            {
                var data = await _context.Votos.ToListAsync();
                Log.Information($"{data}");
                return ApiResult<List<Voto>>.Ok(data);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<List<Voto>>.Fail(ex.Message);
            }
        }

        // GET: api/Votos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Voto>>> GetVoto(Guid id)
        {
            try
            {
                var voto = await _context
                    .Votos
                    .Include(e => e.Eleccion)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (voto == null)
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<Voto>.Fail("Datos no encontrados");
                }
                Log.Information($"{voto}");
                return ApiResult<Voto>.Ok(voto);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Voto>.Fail(ex.Message);
            }
        }

        // PUT: api/Votos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Voto>>> PutVoto(Guid id, Voto voto)
        {
            if (id != voto.Id)
            {
                Log.Information("No coinciden los identificadores");
                return ApiResult<Voto>.Fail("No coinciden los identificadores");
            }

            _context.Entry(voto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!VotoExists(id))
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<Voto>.Fail("Datos no encontrados");
                }
                else
                {
                    Log.Information(ex.Message);
                    return ApiResult<Voto>.Fail(ex.Message);
                }
            }
            Log.Information($"{null}");
            return ApiResult<Voto>.Ok(null);
        }

        // POST: api/Votos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<Voto>>> PostVoto(Voto voto)
        {
            try
            {
                _context.Votos.Add(voto);
                await _context.SaveChangesAsync();
                Log.Information($"{voto}");
                return ApiResult<Voto>.Ok(voto);

            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Voto>.Fail(ex.Message);
            }
        }

        // DELETE: api/Votos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Voto>>> DeleteVoto(Guid id)
        {
            try
            {
                var voto = await _context.Votos.FindAsync(id);
                if (voto == null)
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<Voto>.Fail("Datos no encontrados");
                }

                _context.Votos.Remove(voto);
                await _context.SaveChangesAsync();
                Log.Information($"{null}");
                return ApiResult<Voto>.Ok(null);

            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Voto>.Fail(ex.Message);
            }
        }

        private bool VotoExists(Guid id)
        {
            return _context.Votos.Any(e => e.Id == id);
        }
    }
}
