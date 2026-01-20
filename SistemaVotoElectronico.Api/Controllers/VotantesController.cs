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
    public class VotantesController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public VotantesController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/Votantes
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Votante>>>> GetVotante()
        {
            try
            {
                var data = await _context.Votantes.ToListAsync();
                Log.Information($"{data}");
                return ApiResult<List<Votante>>.Ok(data);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<List<Votante>>.Fail(ex.Message);
            }
        }

        // GET: api/Votantes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Votante>>> GetVotante(int id)
        {
            try
            {
                var votante = await _context
                    .Votantes
                    .Include(e => e.HistorialVotos)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (votante == null)
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<Votante>.Fail("Datos no encontrados");
                }
                Log.Information($"{votante}");
                return ApiResult<Votante>.Ok(votante);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Votante>.Fail(ex.Message);
            }
        }

        // PUT: api/Votantes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Votante>>> PutVotante(int id, Votante votante)
        {
            if (id != votante.Id)
            {
                Log.Information("Los identificadores no coinciden");
                return ApiResult<Votante>.Fail("Los identificadores no coinciden");
            }
            try
            {
                var votanteAnterior = await _context.Votantes
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(x => x.Id == id);

                if (votanteAnterior == null) return ApiResult<Votante>.Fail("Votante no existe");

                if (string.IsNullOrEmpty(votante.Contrasena))
                {
                    votante.Contrasena = votanteAnterior.Contrasena;
                }
                else
                {
                    votante.Contrasena = BCrypt.Net.BCrypt.HashPassword(votante.Contrasena);
                }

                _context.Entry(votante).State = EntityState.Modified;

                await _context.SaveChangesAsync(); 

                return ApiResult<Votante>.Ok(null);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!VotanteExists(id))
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<Votante>.Fail("Datos no encontrados");
                }
                else
                {
                    Log.Information(ex.Message);
                    return ApiResult<Votante>.Fail(ex.Message);
                }
            }
        }

        // POST: api/Votantes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<Votante>>> PostVotante(Votante votante)
        {
            try
            {
                if (!string.IsNullOrEmpty(votante.Contrasena))
                {
                    votante.Contrasena = BCrypt.Net.BCrypt.HashPassword(votante.Contrasena);
                }

                _context.Votantes.Add(votante);
                await _context.SaveChangesAsync();

                Log.Information($"{votante}");
                return ApiResult<Votante>.Ok(votante);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Votante>.Fail(ex.Message);
            }
        }

        // DELETE: api/Votantes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Votante>>> DeleteVotante(int id)
        {
            try
            {
                var votante = await _context.Votantes.FindAsync(id);
                if (votante == null)
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<Votante>.Fail("Datos no encontrados");
                }

                _context.Votantes.Remove(votante);
                await _context.SaveChangesAsync();
                Log.Information($"{null}");
                return ApiResult<Votante>.Ok(null);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Votante>.Fail(ex.Message);
            }
        }

        private bool VotanteExists(int id)
        {
            return _context.Votantes.Any(e => e.Id == id);
        }
    }
}
