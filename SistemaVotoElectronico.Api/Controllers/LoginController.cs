using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotoElectronico.Api.Data;
using SistemaVotoElectronico.Modelos.DTOs;
using SistemaVotoElectronico.Modelos.Responses;

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
        public async Task<ActionResult<ApiResult<LoginResponseDto>>> Login(LoginDto login)
        {
            try
            {
                // 1. Verificar Admin
                var admin = await _context.Administradores.FirstOrDefaultAsync(x => x.Correo == login.Correo);
                if (admin != null && BCrypt.Net.BCrypt.Verify(login.Contrasena, admin.Contrasena))
                {
                    return ApiResult<LoginResponseDto>.Ok(new LoginResponseDto
                    {
                        Id = admin.Id,
                        Nombre = admin.NombreCompleto,
                        Correo = admin.Correo,
                        Rol = "Administrador"
                    });
                }

                // 2. Verificar Jefe de Mesa
                var jefe = await _context.JefesDeMesa.FirstOrDefaultAsync(x => x.Correo == login.Correo);
                if (jefe != null && BCrypt.Net.BCrypt.Verify(login.Contrasena, jefe.Contrasena))
                {
                    return ApiResult<LoginResponseDto>.Ok(new LoginResponseDto
                    {
                        Id = jefe.Id,
                        Nombre = jefe.NombreCompleto,
                        Correo = jefe.Correo,
                        Rol = "JefeDeMesa"
                    });
                }

                // 3. Verificar Candidato
                var candidato = await _context.Candidatos.FirstOrDefaultAsync(x => x.Correo == login.Correo);
                if (candidato != null && BCrypt.Net.BCrypt.Verify(login.Contrasena, candidato.Contrasena))
                {
                    return ApiResult<LoginResponseDto>.Ok(new LoginResponseDto
                    {
                        Id = candidato.Id,
                        Nombre = candidato.NombreCompleto,
                        Correo = candidato.Correo,
                        Rol = "Candidato"
                    });
                }

                // 4. Verificar Votante
                var votante = await _context.Votantes.FirstOrDefaultAsync(x => x.Correo == login.Correo);
                if (votante != null && BCrypt.Net.BCrypt.Verify(login.Contrasena, votante.Contrasena))
                {
                    return ApiResult<LoginResponseDto>.Ok(new LoginResponseDto
                    {
                        Id = votante.Id,
                        Nombre = votante.NombreCompleto,
                        Correo = votante.Correo,
                        Rol = "Votante"
                    });
                }

                return ApiResult<LoginResponseDto>.Fail("Usuario o contraseña incorrectos");
            }
            catch (Exception ex)
            {
                return ApiResult<LoginResponseDto>.Fail(ex.Message);
            }
        }
    }
}