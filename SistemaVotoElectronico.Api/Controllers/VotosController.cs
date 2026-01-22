using Microsoft.AspNetCore.Mvc;
using Serilog;

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
                    .Include(p => p.Votante)
                    .Include(p => p.Eleccion)
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

                try
                {
                    if (registroPadron.Votante != null && !string.IsNullOrEmpty(registroPadron.Votante.Correo))
                    {
                        EnviarCertificadoPDF(
                            registroPadron.Votante.NombreCompleto ?? "Ciudadano",
                            registroPadron.Votante.Correo,
                            registroPadron.Eleccion?.Nombre ?? "Elección General",
                            DateTime.Now
                        );
                    }
                }
                catch (Exception exEmail)
                {
                    Log.Error($"Voto guardado, pero error al enviar email: {exEmail.Message}");
                }

                return ApiResult<string>.Ok("Su voto ha sido registrado correctamente.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error($"Error crítico al emitir voto: {ex.Message}");
                return ApiResult<string>.Fail("Ocurrió un error interno al procesar su voto. Intente nuevamente.");
            }
        }

        private void EnviarCertificadoPDF(string nombreVotante, string correoDestino, string nombreEleccion, DateTime fecha)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                PdfFont fuenteNegrita = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont fuenteNormal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                document.Add(new Paragraph("CERTIFICADO DE VOTACIÓN DIGITAL")
                    .SetFont(fuenteNegrita) 
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20));

                document.Add(new Paragraph("\n\n"));

                document.Add(new Paragraph("El Sistema de Voto Electrónico certifica que el ciudadano/a:")
                    .SetFont(fuenteNormal));

                document.Add(new Paragraph()
                    .Add(new Text(nombreVotante.ToUpper())
                        .SetFont(fuenteNegrita) 
                        .SetFontColor(ColorConstants.BLUE))
                    .SetFontSize(16)
                    .SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph("\nHa ejercido exitosamente su derecho al voto en:")
                    .SetFont(fuenteNormal));

                document.Add(new Paragraph(nombreEleccion)
                    .SetFont(fuenteNegrita) 
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph($"\nFecha y Hora de registro: {fecha:dd/MM/yyyy HH:mm:ss}")
                    .SetFont(fuenteNormal));

                document.Add(new Paragraph($"Identificador de Transacción: {Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}")
                    .SetFont(fuenteNormal)
                    .SetFontSize(10));

                document.Add(new Paragraph("\n\nGracias por fortalecer la democracia.")
                    .SetFont(fuenteNormal) 
                    .SetTextAlignment(TextAlignment.CENTER)); 

                document.Close();

                byte[] bytesPdf = stream.ToArray();

                string miCorreo = "bastidaspaul83@gmail.com";
                string miPassword = "njkb gyyh qygc wviw";

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(miCorreo, miPassword),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(miCorreo, "Sistema Voto Electrónico"),
                    Subject = "Constancia de Votación",
                    Body = $"Estimado/a {nombreVotante},<br/><br/>Su voto ha sido procesado correctamente.<br/>Adjunto encontrará su certificado digital de votación.<br/><br/>Atentamente,<br/>Consejo Electoral.",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(correoDestino);
                mailMessage.Attachments.Add(new Attachment(new MemoryStream(bytesPdf), "Certificado_Votacion.pdf"));

                smtpClient.Send(mailMessage);
            }
        }

        // GET: api/Votos
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Voto>>>> GetVotos()
        {
            try
            {
                var data = await _context.Votos.ToListAsync();
                return ApiResult<List<Voto>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Voto>>.Fail(ex.Message);
            }
        }

        // GET: api/Votos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Voto>>> GetVoto(Guid id)
        {
            try
            {
                var voto = await _context.Votos
                    .Include(e => e.Eleccion)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (voto == null) return ApiResult<Voto>.Fail("Datos no encontrados");

                return ApiResult<Voto>.Ok(voto);
            }
            catch (Exception ex)
            {
                return ApiResult<Voto>.Fail(ex.Message);
            }
        }

        // POST: api/Votos 
        [HttpPost]
        public async Task<ActionResult<ApiResult<Voto>>> PostVoto(Voto voto)
        {
            try
            {
                _context.Votos.Add(voto);
                await _context.SaveChangesAsync();
                return ApiResult<Voto>.Ok(voto);
            }
            catch (Exception ex)
            {
                return ApiResult<Voto>.Fail(ex.Message);
            }
        }

        // PUT: api/Votos/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Voto>>> PutVoto(Guid id, Voto voto)
        {
            if (id != voto.Id) return ApiResult<Voto>.Fail("No coinciden los identificadores");

            _context.Entry(voto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!VotoExists(id)) return ApiResult<Voto>.Fail("Datos no encontrados");
                else return ApiResult<Voto>.Fail(ex.Message);
            }

            return ApiResult<Voto>.Ok(null);
        }

        // DELETE: api/Votos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Voto>>> DeleteVoto(Guid id)
        {
            try
            {
                var voto = await _context.Votos.FindAsync(id);
                if (voto == null) return ApiResult<Voto>.Fail("Datos no encontrados");

                _context.Votos.Remove(voto);
                await _context.SaveChangesAsync();
                return ApiResult<Voto>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Voto>.Fail(ex.Message);
            }
        }

        private bool VotoExists(Guid id)
        {
            return _context.Votos.Any(e => e.Id == id);
        }
    }
}
