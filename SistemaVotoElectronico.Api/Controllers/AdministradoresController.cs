using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                var data = await _context.Administradores.ToListAsync();
                return ApiResult<List<Administrador>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Administrador>>.Fail(ex.Message);
            }
            
        }

        // GET: api/Administradores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Administrador>>> GetAdministrador(int id)
        {
            try
            {
                var administrador = await _context.Administradores.FindAsync(id);

                if (administrador == null)
                {
                    return ApiResult<Administrador>.Fail("Datos no encontrados");
                }

                return ApiResult<Administrador>.Ok(administrador);
            }
            catch (Exception ex)
            {
                return ApiResult<Administrador>.Fail(ex.Message);
            }
            
        }

        // PUT: api/Administradores/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Administrador>>> PutAdministrador(int id, Administrador administrador)
        {
            if (id != administrador.Id)
            {
                return ApiResult<Administrador>.Fail("No coinciden los identificadores");
            }

            _context.Entry(administrador).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!AdministradorExists(id))
                {
                    return ApiResult<Administrador>.Fail("Datos no encontrados");
                }
                else
                {
                    return ApiResult<Administrador>.Fail(ex.Message);
                }
            }

            return ApiResult<Administrador>.Ok(null);
        }

        // POST: api/Administradores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<Administrador>>> PostAdministrador(Administrador administrador)
        {
            try
            {
                _context.Administradores.Add(administrador);
                await _context.SaveChangesAsync();

                return ApiResult<Administrador>.Ok(administrador);
            }
            catch (Exception ex)
            {
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
                    return ApiResult<Administrador>.Fail("Datos no encontrados");
                }

                _context.Administradores.Remove(administrador);
                await _context.SaveChangesAsync();

                return ApiResult<Administrador>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Administrador>.Fail(ex.Message);
            }
        }

        private bool AdministradorExists(int id)
        {
            return _context.Administradores.Any(e => e.Id == id);
        }
    }
}
