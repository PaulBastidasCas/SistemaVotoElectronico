using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotoElectronico.Api.Data;
using SistemaVotoElectronico.Modelos.Entidades;
using SistemaVotoElectronico.Modelos.Responses;
using SistemaVotoElectronico.Modelos.DTOs;
using System.Net;
using System.Net.Mail;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;
        private const string SmtpCorreo = "bastidaspaul83@gmail.com";
        private const string SmtpPassword = "njkb gyyh qygc wviw";

        public AuthController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        [HttpPost("Recuperar")]
        public async Task<ActionResult<ApiResult<bool>>> SolicitarRecuperacion(RecuperarDto dto)
        {
            // 1. Verificar si existe el correo
            bool existe = await _context.Administradores.AnyAsync(x => x.Correo == dto.Correo) ||
                          await _context.JefesDeMesa.AnyAsync(x => x.Correo == dto.Correo) ||
                          await _context.Candidatos.AnyAsync(x => x.Correo == dto.Correo) ||
                          await _context.Votantes.AnyAsync(x => x.Correo == dto.Correo);

            if (!existe) return ApiResult<bool>.Fail("El correo no está registrado.");

            // 2. Generar token alfanumérico de 6 caracteres
            var token = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            //3.Crear y guardar registro de solicitud de recuperación
            var solicitud = new SolicitudRecuperacion
            {
                Correo = dto.Correo,
                Token = token,
                Expiracion = DateTime.UtcNow.AddMinutes(15),
                Usado = false
            };

            _context.SolicitudesRecuperacion.Add(solicitud);
            await _context.SaveChangesAsync();

            // 4. Intentar enviar correo; si falla, emitir token por consola
            try
            {
                await EnviarCorreoAsync(dto.Correo, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine($"[CORREO FALLIDO POR BLOQUEO DE PUERTO]");
                Console.WriteLine($"PARA RECUPERAR LA CUENTA USA ESTE TOKEN: {token}");
                Console.WriteLine("---------------------------------------------------");

                return ApiResult<bool>.Fail($"Error enviando correo (Revise Logs): {ex.Message}");
            }

            return ApiResult<bool>.Ok(true);
        }

        [HttpPost("Reset")]
        public async Task<ActionResult<ApiResult<bool>>> CambiarContrasena(ResetPasswordDto dto)
        {
            // 1. Buscar la última solicitud válida y no usada para el token
            var solicitud = await _context.SolicitudesRecuperacion
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(x => x.Token == dto.Token && !x.Usado);

            // 2. Validar token y tiempo de expiración
            if (solicitud == null) return ApiResult<bool>.Fail("Token inválido o ya utilizado.");
            if (solicitud.Expiracion < DateTime.UtcNow) return ApiResult<bool>.Fail("El token ha expirado.");

            // 3. Hashear la nueva contraseña
            string passHash = BCrypt.Net.BCrypt.HashPassword(dto.NuevaContrasena);
            bool actualizado = false;

            // 4. Buscar secuencialmente en las entidades de usuarios y actualizar si se encuentra
            var admin = await _context.Administradores.FirstOrDefaultAsync(x => x.Correo == solicitud.Correo);
            if (admin != null) { admin.Contrasena = passHash; actualizado = true; }

            if (!actualizado)
            {
                var jefe = await _context.JefesDeMesa.FirstOrDefaultAsync(x => x.Correo == solicitud.Correo);
                if (jefe != null) { jefe.Contrasena = passHash; actualizado = true; }
            }

            if (!actualizado)
            {
                var cand = await _context.Candidatos.FirstOrDefaultAsync(x => x.Correo == solicitud.Correo);
                if (cand != null) { cand.Contrasena = passHash; actualizado = true; }
            }

            if (!actualizado)
            {
                var votante = await _context.Votantes.FirstOrDefaultAsync(x => x.Correo == solicitud.Correo);
                if (votante != null) { votante.Contrasena = passHash; actualizado = true; }
            }

            // 5. Marcar token como usado y confirmar guardado
            if (actualizado)
            {
                solicitud.Usado = true;
                await _context.SaveChangesAsync();
                return ApiResult<bool>.Ok(true);
            }

            return ApiResult<bool>.Fail("No se encontró el usuario asociado al token.");
        }

        private async Task EnviarCorreoAsync(string destino, string token)
        {
            // 1. Configurar cliente SMTP de Gmail
            var cliente = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(SmtpCorreo, SmtpPassword),
                Timeout = 5000 
            };

            // 2. Construir mensaje y enviar
            var mensaje = new MailMessage(SmtpCorreo, destino, "Recuperación de Contraseña",
                $"Token: {token}");

            await cliente.SendMailAsync(mensaje);
        }
    }
}