using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SistemaVotoElectronico.Api.Data;
using SistemaVotoElectronico.Modelos;
using SistemaVotoElectronico.Modelos.DTOs;
using SistemaVotoElectronico.Modelos.Entidades;
using SistemaVotoElectronico.Modelos.Responses;

// For creating PDF
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Colors;
using System.Net.Mail;
using System.Net;
using iText.Kernel.Geom;

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
                    .FirstOrDefaultAsync(p => p.CodigoEnlace == request.CodigoEnlace && p.EleccionId == request.EleccionId);

                if (registroPadron == null)
                {
                    Log.Error("[ERROR] Codigo no encontrado en BD"); 
                    return ApiResult<string>.Fail("Codigo invalido.");
                }

                if (registroPadron.CodigoCanjeado)
                {
                    Log.Error("[ERROR] Codigo ya fue canjeado"); 
                    return ApiResult<string>.Fail("Codigo ya usado.");
                }

                var nuevoVoto = new Voto
                {
                    Id = Guid.NewGuid(),
                    EleccionId = request.EleccionId,
                    ListaPresidenteId = request.ListaPresidenteId,
                    ListaAsambleistaId = request.ListaAsambleistaId,
                    FechaRegistro = DateTime.Now
                };

                _context.Votos.Add(nuevoVoto);
                registroPadron.CodigoCanjeado = true;
                registroPadron.FechaVoto = DateTime.Now;
                
                // Actualizamos indicadores (informativo)
                registroPadron.VotoPlanchaRealizado = (request.ListaPresidenteId == request.ListaAsambleistaId && request.ListaPresidenteId != null);
                registroPadron.VotoNominalRealizado = (request.ListaPresidenteId != request.ListaAsambleistaId);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                Log.Information("[EXITO] Voto guardado en BD. Preparando correo..."); 

                if (registroPadron.Votante != null && !string.IsNullOrEmpty(registroPadron.Votante.Correo))
                {
                    Log.Information($"[INFO] Enviando correo a: {registroPadron.Votante.Correo}"); 

                    var datos = new
                    {
                        Nombre = registroPadron.Votante.NombreCompleto,
                        Correo = registroPadron.Votante.Correo,
                        Eleccion = registroPadron.Eleccion?.Nombre,
                        Fecha = DateTime.Now
                    };

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            Log.Information("[ASYNC] Iniciando envio SMTP...");
                            await EnviarCertificadoPDF(datos.Nombre, datos.Correo, datos.Eleccion, datos.Fecha);
                            Log.Information("[ASYNC] Correo enviado con exito!"); 
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"[ASYNC ERROR] Fallo el envio: {ex.Message}");
                            if (ex.InnerException != null) Log.Error($"[INNER EX] {ex.InnerException.Message}");
                        }
                    });
                }
                else
                {
                    Log.Information("[AVISO] El votante no tiene correo o es nulo. No se envia nada.");
                }

                return ApiResult<string>.Ok("Voto registrado.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error($"[CRITICAL] Excepcion en transaccion: {ex.Message}");
                return ApiResult<string>.Fail("Error interno.");
            }
        }

        private async Task EnviarCertificadoPDF(string nombreVotante, string correoDestino, string nombreEleccion, DateTime fecha)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                PdfFont fuenteNegrita = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont fuenteNormal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                document.Add(new Paragraph("CERTIFICADO DE VOTACION DIGITAL")
                    .SetFont(fuenteNegrita)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20));

                document.Add(new Paragraph("\n\n"));

                document.Add(new Paragraph("El Sistema de Voto Electronico certifica que el ciudadano/a:")
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

                document.Add(new Paragraph($"Identificador de Transaccion: {Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}")
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
                    From = new MailAddress(miCorreo, "Sistema Voto Electronico"),
                    Subject = "Constancia de Votacion",
                    Body = $"Estimado/a {nombreVotante},<br/><br/>Su voto ha sido procesado correctamente.<br/>Adjunto encontrara su certificado digital de votacion.<br/><br/>Atentamente,<br/>Consejo Electoral.",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(correoDestino);
                mailMessage.Attachments.Add(new Attachment(new MemoryStream(bytesPdf), "Certificado_Votacion.pdf"));

                smtpClient.Send(mailMessage);
            }
        }

        [HttpGet("resultados/{eleccionId}")]
        public async Task<ActionResult<ApiResult<ResultadoEleccionDto>>> GetResultados(int eleccionId, int escanosA_Repartir = 5)
        {
            try
            {
                var eleccion = await _context.Elecciones.FindAsync(eleccionId);
                if (eleccion == null) return ApiResult<ResultadoEleccionDto>.Fail("Eleccion no encontrada");

                var votos = await _context.Votos
                    .AsNoTracking()
                    .Where(v => v.EleccionId == eleccionId) 
                    .ToListAsync();

                var listas = await _context.ListaElectorales
                    .AsNoTracking()
                    .Where(l => l.EleccionId == eleccionId)
                    .ToListAsync();

                // 1. RESULTADOS PRESIDENTE/VICEPRESIDENTE
                var resultadosPresidente = new List<DetalleListaDto>();
                int totalVotosPresidente = 0;

                foreach (var lista in listas)
                {
                    int totalLista = votos.Count(v => v.ListaPresidenteId == lista.Id);
                    totalVotosPresidente += totalLista;

                    resultadosPresidente.Add(new DetalleListaDto
                    {
                        Lista = lista.Nombre,
                        Siglas = lista.Siglas,
                        Color = !string.IsNullOrEmpty(lista.Color) ? lista.Color : "#6c757d",
                        VotosTotales = totalLista,
                        EscanosAsignados = 0 
                    });
                }

                if (totalVotosPresidente > 0)
                {
                    resultadosPresidente.ForEach(r => r.Porcentaje = Math.Round(((double)r.VotosTotales / totalVotosPresidente) * 100, 2));
                }

                // 2. RESULTADOS ASAMBLEISTAS (WEBSTER)
                var resultadosAsambleistas = new List<DetalleListaDto>();
                int totalVotosAsambleistas = 0;

                foreach (var lista in listas)
                {
                    int totalLista = votos.Count(v => v.ListaAsambleistaId == lista.Id);
                    totalVotosAsambleistas += totalLista;

                    resultadosAsambleistas.Add(new DetalleListaDto
                    {
                        Lista = lista.Nombre,
                        Siglas = lista.Siglas,
                        Color = !string.IsNullOrEmpty(lista.Color) ? lista.Color : "#6c757d",
                        VotosTotales = totalLista,
                        EscanosAsignados = 0
                    });
                }

                if (totalVotosAsambleistas > 0)
                {
                    resultadosAsambleistas.ForEach(r => r.Porcentaje = Math.Round(((double)r.VotosTotales / totalVotosAsambleistas) * 100, 2));
                }

                // Reparticion Webster Solo para Asambleistas
                var cocientes = new List<dynamic>();
                foreach (var item in resultadosAsambleistas)
                {
                    for (int i = 1; i <= escanosA_Repartir * 2; i += 2)
                    {
                        cocientes.Add(new { Lista = item, Valor = (double)item.VotosTotales / i });
                    }
                }

                var escanosGanadores = cocientes.OrderByDescending(x => x.Valor).Take(escanosA_Repartir).ToList();
                foreach (var ganador in escanosGanadores)
                {
                    DetalleListaDto listaGanadora = ganador.Lista;
                    listaGanadora.EscanosAsignados++;
                }

                var reporte = new ResultadoEleccionDto
                {
                    Eleccion = eleccion.Nombre,
                    TotalVotosPresidente = totalVotosPresidente,
                    TotalVotosAsambleistas = totalVotosAsambleistas,
                    TotalEscanos = escanosA_Repartir,
                    FechaCorte = DateTime.Now,
                    ResultadosPresidente = resultadosPresidente.OrderByDescending(r => r.VotosTotales).ToList(),
                    ResultadosAsambleistas = resultadosAsambleistas.OrderByDescending(r => r.VotosTotales).ToList()
                };

                return ApiResult<ResultadoEleccionDto>.Ok(reporte);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return ApiResult<ResultadoEleccionDto>.Fail("Error: " + ex.Message);
            }
        }

        [HttpGet("reporte-pdf/{eleccionId}")]
        public async Task<IActionResult> GetReportePdf(int eleccionId)
        {
            var actionResult = await GetResultados(eleccionId);
            ApiResult<ResultadoEleccionDto> apiResult = actionResult.Value;

            if (apiResult == null && actionResult.Result is ObjectResult objResult)
            {
                apiResult = objResult.Value as ApiResult<ResultadoEleccionDto>;
            }

            if (apiResult != null && apiResult.Success && apiResult.Data != null)
            {
                var data = apiResult.Data;
                try
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        PdfWriter writer = new PdfWriter(stream);
                        PdfDocument pdf = new PdfDocument(writer);
                        Document document = new Document(pdf);

                        PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                        PdfFont normal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                        document.Add(new Paragraph("REPORTE DE RESULTADOS")
                            .SetFont(bold).SetFontSize(18).SetTextAlignment(TextAlignment.CENTER));
                        document.Add(new Paragraph(data.Eleccion)
                            .SetFont(bold).SetFontSize(14).SetTextAlignment(TextAlignment.CENTER));
                        document.Add(new Paragraph($"Corte: {data.FechaCorte}")
                            .SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                        
                        // TABLA 1: PRESIDENTES
                        document.Add(new Paragraph("\nRESULTADOS PRESIDENCIALES").SetFont(bold));
                        document.Add(CrearTablaResultados(data.ResultadosPresidente, bold, normal));
                        document.Add(new Paragraph($"Total Votos Vlidos: {data.TotalVotosPresidente.ToString("N0")}").SetFont(bold));

                        // TABLA 2: ASAMBLEISTAS
                        document.Add(new Paragraph("\n\nRESULTADOS ASAMBLEISTAS").SetFont(bold));
                        document.Add(CrearTablaResultados(data.ResultadosAsambleistas, bold, normal));
                        document.Add(new Paragraph($"Total Votos Vlidos: {data.TotalVotosAsambleistas.ToString("N0")}").SetFont(bold));

                        document.Close();
                        return File(stream.ToArray(), "application/pdf", $"Resultados_{eleccionId}.pdf");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error interno creando PDF: {ex.Message}");
                }
            }
            return BadRequest($"No se pudieron obtener los datos.");
        }

        private Table CrearTablaResultados(List<DetalleListaDto> resultados, PdfFont bold, PdfFont normal)
        {
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 2, 2, 2, 2 })).UseAllAvailableWidth();

            table.AddHeaderCell(new Cell().Add(new Paragraph("Organizacin").SetFont(bold).SetFontColor(ColorConstants.WHITE))).SetBackgroundColor(ColorConstants.DARK_GRAY);
            table.AddHeaderCell(new Cell().Add(new Paragraph("Siglas").SetFont(bold).SetFontColor(ColorConstants.WHITE))).SetBackgroundColor(ColorConstants.DARK_GRAY);
            table.AddHeaderCell(new Cell().Add(new Paragraph("Votos").SetFont(bold).SetFontColor(ColorConstants.WHITE))).SetBackgroundColor(ColorConstants.DARK_GRAY);
            table.AddHeaderCell(new Cell().Add(new Paragraph("%").SetFont(bold).SetFontColor(ColorConstants.WHITE))).SetBackgroundColor(ColorConstants.DARK_GRAY);
            table.AddHeaderCell(new Cell().Add(new Paragraph("Escaos").SetFont(bold).SetFontColor(ColorConstants.WHITE))).SetBackgroundColor(ColorConstants.DARK_GRAY);

            foreach (var item in resultados)
            {
                table.AddCell(new Paragraph(item.Lista).SetFont(normal));
                table.AddCell(new Paragraph(item.Siglas).SetFont(normal));
                table.AddCell(new Paragraph(item.VotosTotales.ToString("N0")).SetFont(normal));
                table.AddCell(new Paragraph(item.Porcentaje + "%").SetFont(normal));
                
                var celdaEscanos = new Cell().Add(new Paragraph(item.EscanosAsignados.ToString()).SetFont(bold));
                if (item.EscanosAsignados > 0) celdaEscanos.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                table.AddCell(celdaEscanos);
            }
            return table;
        }
        
        // GET: api/Votos
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Voto>>>> GetVotos()
        {
            try
            {
                var data = await _context.Votos.AsNoTracking().ToListAsync();
                return ApiResult<List<Voto>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Voto>>.Fail(ex.Message);
            }
        }

        // GET: api/Votos/GUID
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Voto>>> GetVoto(Guid id)
        {
            try
            {
                var voto = await _context.Votos.AsNoTracking().Include(e => e.Eleccion).FirstOrDefaultAsync(e => e.Id == id);
                if (voto == null) return ApiResult<Voto>.Fail("Datos no encontrados");
                return ApiResult<Voto>.Ok(voto);
            }
            catch (Exception ex)
            {
                return ApiResult<Voto>.Fail(ex.Message);
            }
        }

        // DELETE: api/Votos/GUID
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

        // PUT: api/Votos/GUID
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Voto>>> PutVoto(Guid id, Voto voto)
        {
            if (id != voto.Id) return ApiResult<Voto>.Fail("IDs no coinciden");
            _context.Entry(voto).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (Exception ex) { return ApiResult<Voto>.Fail(ex.Message); }
            return ApiResult<Voto>.Ok(null);
        }
    }
}
