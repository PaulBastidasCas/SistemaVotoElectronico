using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public LoginController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<object>>> Login(LoginDto login)
        {
            try
            {
                var admin = await _context.Administradores.FirstOrDefaultAsync(x => x.Correo == login.Correo);
                if (admin != null && BCrypt.Net.BCrypt.Verify(login.Contrasena, admin.Contrasena))
                {
                    return ApiResult<object>.Ok(new { Nombre = admin.NombreCompleto, Correo = admin.Correo, Rol = "Administrador", Id = admin.Id });
                }

                var jefe = await _context.JefesDeMesa.FirstOrDefaultAsync(x => x.Correo == login.Correo);
                if (jefe != null && BCrypt.Net.BCrypt.Verify(login.Contrasena, jefe.Contrasena))
                {
                    return ApiResult<object>.Ok(new { Nombre = jefe.NombreCompleto, Correo = jefe.Correo, Rol = "JefeDeMesa", Id = jefe.Id });
                }

                var candidato = await _context.Candidatos.FirstOrDefaultAsync(x => x.Correo == login.Correo);
                if (candidato != null && BCrypt.Net.BCrypt.Verify(login.Contrasena, candidato.Contrasena))
                {
                    return ApiResult<object>.Ok(new { Nombre = candidato.NombreCompleto, Correo = "Candidato", Rol = "Candidato", Id = candidato.Id });
                }

                var votante = await _context.Votantes.FirstOrDefaultAsync(x => x.Correo == login.Correo);
                if (votante != null && BCrypt.Net.BCrypt.Verify(login.Contrasena, votante.Contrasena))
                {
                    return ApiResult<object>.Ok(new { Nombre = votante.NombreCompleto, Correo = votante.Correo, Rol = "Votante", Id = votante.Id });
                }

                return ApiResult<object>.Fail("Usuario o contraseña incorrectos");
            }
            catch (Exception ex)
            {
                return ApiResult<object>.Fail(ex.Message);
            }
        }
    }
}