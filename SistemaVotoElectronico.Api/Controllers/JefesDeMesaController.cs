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
    public class JefesDeMesaController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public JefesDeMesaController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/JefesDeMesa
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<JefeDeMesa>>>> GetAll()
        {
            var data = await _context.JefesDeMesa.Include(j => j.MesaAsignada).ToListAsync();
            return ApiResult<List<JefeDeMesa>>.Ok(data);
        }

        // GET: api/JefesDeMesa/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<JefeDeMesa>>> Get(int id)
        {
            var jefe = await _context.JefesDeMesa.Include(j => j.MesaAsignada).FirstOrDefaultAsync(x => x.Id == id);
            if (jefe == null) return ApiResult<JefeDeMesa>.Fail("No encontrado");
            return ApiResult<JefeDeMesa>.Ok(jefe);
        }

        // PUT: api/JefesDeMesa/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<JefeDeMesa>>> Put(int id, JefeDeMesa jefe)
        {
            if (id != jefe.Id) return ApiResult<JefeDeMesa>.Fail("ID incorrecto");

            var anterior = await _context.JefesDeMesa.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (anterior == null) return ApiResult<JefeDeMesa>.Fail("No existe");

            if (string.IsNullOrEmpty(jefe.Contrasena)) jefe.Contrasena = anterior.Contrasena;
            else jefe.Contrasena = BCrypt.Net.BCrypt.HashPassword(jefe.Contrasena);

            _context.Entry(jefe).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ApiResult<JefeDeMesa>.Ok(null);
        }

        // POST: api/JefesDeMesa
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<JefeDeMesa>>> Post(JefeDeMesa jefe)
        {
            try
            {
                if (!string.IsNullOrEmpty(jefe.Contrasena))
                    jefe.Contrasena = BCrypt.Net.BCrypt.HashPassword(jefe.Contrasena);

                _context.JefesDeMesa.Add(jefe);
                await _context.SaveChangesAsync();
                return ApiResult<JefeDeMesa>.Ok(jefe);
            }
            catch (Exception ex) { return ApiResult<JefeDeMesa>.Fail(ex.Message); }
        }

        // DELETE: api/JefesDeMesa/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<bool>>> Delete(int id)
        {
            var jefe = await _context.JefesDeMesa.FindAsync(id);
            if (jefe == null) return ApiResult<bool>.Fail("No encontrado");
            _context.JefesDeMesa.Remove(jefe);
            await _context.SaveChangesAsync();
            return ApiResult<bool>.Ok(true);
        }
    }
}
