using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SistemaVotoElectronico.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: api/Votos
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Voto>>>> GetVoto()
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
                Log.Information("Los identificadores no coinciden");
                return ApiResult<Voto>.Fail("Los identificadores no coinciden");
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
        public async Task<ActionResult<ApiResult<Voto>>> PostVoto([FromBody] VotoRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var registroPadron = await _context.PadronElectorales
                    .FirstOrDefaultAsync(p => p.CodigoEnlace == request.CodigoEnlace
                                           && p.EleccionId == request.EleccionId);

                if (registroPadron == null)
                    return ApiResult<Voto>.Fail("Código de votación inválido.");

                if (registroPadron.CodigoCanjeado)
                    return ApiResult<Voto>.Fail("Este código ya fue utilizado. No se puede votar dos veces.");

                var nuevoVoto = new Voto
                {
                    EleccionId = request.EleccionId,
                    IdCandidatoSeleccionado = request.IdCandidatoSeleccionado, 
                    IdListaSeleccionada = request.IdListaSeleccionada,
                    FechaRegistro = DateTime.Now
                };

                _context.Votos.Add(nuevoVoto);

                registroPadron.CodigoCanjeado = true;
                registroPadron.FechaVoto = DateTime.Now;

                if (request.IdListaSeleccionada != null) registroPadron.VotoPlanchaRealizado = true;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return ApiResult<Voto>.Ok(nuevoVoto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResult<Voto>.Fail("Error al registrar voto: " + ex.Message);
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

    public class VotoRequest
    {
        public string CodigoEnlace { get; set; } 
        public int EleccionId { get; set; }
        public int? IdCandidatoSeleccionado { get; set; }
        public int? IdListaSeleccionada { get; set; }
    }
}
