using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SistemaVotoElectronico.Api.Data;
using SistemaVotoElectronico.Modelos.Entidades;
using SistemaVotoElectronico.Modelos.Responses;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministradoresController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public AdministradoresController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/Administradores
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Administrador>>>> GetAdministrador()
        {
            try
            {
                var data = await _context.Administradores.AsNoTracking().ToListAsync();
                Log.Information($"{data}");
                return ApiResult<List<Administrador>>.Ok(data);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<List<Administrador>>.Fail(ex.Message);
            }
            
        }

        // GET: api/Administradores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Administrador>>> GetAdministrador(int id)
        {
            try
            {
                var administrador = await _context.Administradores.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

                if (administrador == null)
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<Administrador>.Fail("Datos no encontrados");
                }
                Log.Information($"{administrador}");
                return ApiResult<Administrador>.Ok(administrador);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Administrador>.Fail(ex.Message);
            }
            
        }

        // PUT: api/Administradores/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Administrador>>> PutAdministrador(int id, Administrador administrador)
        {
            if (id != administrador.Id) return ApiResult<Administrador>.Fail("ID no coincide");

            try
            {
                var original = await _context.Administradores.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (original == null) return ApiResult<Administrador>.Fail("No encontrado");

                if (string.IsNullOrEmpty(administrador.Contrasena))
                {
                    administrador.Contrasena = original.Contrasena;
                }
                else
                {
                    administrador.Contrasena = BCrypt.Net.BCrypt.HashPassword(administrador.Contrasena);
                }

                _context.Entry(administrador).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return ApiResult<Administrador>.Ok(null);
            }
            catch (Exception ex) { return ApiResult<Administrador>.Fail(ex.Message); }
        }

        // POST: api/Administradores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<Administrador>>> PostAdministrador(Administrador administrador)
        {
            try
            {
                if (string.IsNullOrEmpty(administrador.Contrasena))
                    return ApiResult<Administrador>.Fail("La contraseña es obligatoria");

                administrador.Contrasena = BCrypt.Net.BCrypt.HashPassword(administrador.Contrasena);

                _context.Administradores.Add(administrador);
                await _context.SaveChangesAsync();

                Log.Information($"{administrador}");
                return ApiResult<Administrador>.Ok(administrador);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Administrador>.Fail(ex.Message);
            }
            
        }

        // DELETE: api/Administradores/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Administrador>>> DeleteAdministrador(int id)
        {
            try
            {
                var administrador = await _context.Administradores.FindAsync(id);
                if (administrador == null)
                {
                    Log.Information("Datos no encontrados");
                    return ApiResult<Administrador>.Fail("Datos no encontrados");
                }

                _context.Administradores.Remove(administrador);
                await _context.SaveChangesAsync();
                Log.Information($"{null}");
                return ApiResult<Administrador>.Ok(null);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return ApiResult<Administrador>.Fail(ex.Message);
            }
        }

        private bool AdministradorExists(int id)
        {
            return _context.Administradores.Any(e => e.Id == id);
        }
    }
}
