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
    public class ListaElectoralesController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public ListaElectoralesController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/ListaElectorales
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<ListaElectoral>>>> GetListaElectoral()
        {
            try
            {
                var data = await _context.ListaElectorales.ToListAsync();
                return ApiResult<List<ListaElectoral>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<ListaElectoral>>.Fail(ex.Message);
            }
            
        }

        // GET: api/ListaElectorales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<ListaElectoral>>> GetListaElectoral(int id)
        {
            try
            {
                var listaElectoral = await _context
                    .ListaElectorales
                    .Include(e => e.Eleccion)
                    .Include(e => e.Candidatos)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (listaElectoral == null)
                {
                    return ApiResult<ListaElectoral>.Fail("Datos no encontrados");
                }

                return ApiResult<ListaElectoral>.Ok(listaElectoral);
            }
            catch (Exception ex)
            {
                return ApiResult<ListaElectoral>.Fail(ex.Message);
            }
        }

        // PUT: api/ListaElectorales/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<ListaElectoral>>> PutListaElectoral(int id, ListaElectoral listaElectoral)
        {
            if (id != listaElectoral.Id)
            {
                return ApiResult<ListaElectoral>.Fail("Los identificadores no coinciden");
            }

            _context.Entry(listaElectoral).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ListaElectoralExists(id))
                {
                    return ApiResult<ListaElectoral>.Fail("Datos no encontrados");
                }
                else
                {
                    return ApiResult<ListaElectoral>.Fail(ex.Message);
                }
            }

            return ApiResult<ListaElectoral>.Ok(null);
        }

        // POST: api/ListaElectorales
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<ListaElectoral>>> PostListaElectoral(ListaElectoral listaElectoral)
        {
            try
            {
                _context.ListaElectorales.Add(listaElectoral);
                await _context.SaveChangesAsync();

                return ApiResult<ListaElectoral>.Ok(listaElectoral);
            }
            catch (Exception ex)
            {
                return ApiResult<ListaElectoral>.Fail(ex.Message);
            }
        }

        // DELETE: api/ListaElectorales/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<ListaElectoral>>> DeleteListaElectoral(int id)
        {
            try
            {
                var listaElectoral = await _context.ListaElectorales.FindAsync(id);
                if (listaElectoral == null)
                {
                    return ApiResult<ListaElectoral>.Fail("Datos no encontrados");
                }

                _context.ListaElectorales.Remove(listaElectoral);
                await _context.SaveChangesAsync();

                return ApiResult<ListaElectoral>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<ListaElectoral>.Fail(ex.Message);
            }           
        }

        private bool ListaElectoralExists(int id)
        {
            return _context.ListaElectorales.Any(e => e.Id == id);
        }
    }
}
