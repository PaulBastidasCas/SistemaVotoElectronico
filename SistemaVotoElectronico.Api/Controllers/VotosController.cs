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

        [HttpGet("resultados/{eleccionId}")]
        public async Task<ActionResult<ApiResult<ResultadoEleccionDto>>> GetResultados(int eleccionId, int escanosA_Repartir = 5)
        {
            try
            {
                var eleccion = await _context.Elecciones.FindAsync(eleccionId);
                if (eleccion == null) return ApiResult<ResultadoEleccionDto>.Fail("Elección no encontrada");

                var votos = await _context.Votos
                    .Include(v => v.Eleccion)
                    .Where(v => v.EleccionId == eleccionId)
                    .ToListAsync();

                var listas = await _context.ListaElectorales
                    .Include(l => l.Candidatos)
                    .Where(l => l.EleccionId == eleccionId)
                    .ToListAsync();

                var resultadosListas = new List<DetalleListaDto>();
                int totalVotosValidos = 0;

                foreach (var lista in listas)
                {
                    int votosPlancha = votos.Count(v => v.IdListaSeleccionada == lista.Id);
                    var idsCandidatos = lista.Candidatos.Select(c => c.Id).ToList();
                    int votosNominales = votos.Count(v => v.IdCandidatoSeleccionado.HasValue && idsCandidatos.Contains(v.IdCandidatoSeleccionado.Value));

                    int totalLista = votosPlancha + votosNominales;
                    totalVotosValidos += totalLista;

                    resultadosListas.Add(new DetalleListaDto
                    {
                        Lista = lista.Nombre,
                        Siglas = lista.Siglas,
                        Color = lista.Color ?? "#808080",
                        VotosTotales = totalLista,
                        EscanosAsignados = 0
                    });
                }

                if (totalVotosValidos > 0)
                {
                    resultadosListas.ForEach(r => r.Porcentaje = Math.Round(((double)r.VotosTotales / totalVotosValidos) * 100, 2));
                }

                var cocientes = new List<dynamic>();
                foreach (var item in resultadosListas)
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
                    TotalVotos = totalVotosValidos,
                    TotalEscanos = escanosA_Repartir,
                    FechaCorte = DateTime.Now,
                    Resultados = resultadosListas.OrderByDescending(r => r.VotosTotales).ToList()
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

                        document.Add(new Paragraph("REPORTE DE RESULTADOS OFICIALES")
                            .SetFont(bold).SetFontSize(18).SetTextAlignment(TextAlignment.CENTER));

                        document.Add(new Paragraph(data.Eleccion)
                            .SetFont(bold).SetFontSize(14).SetTextAlignment(TextAlignment.CENTER));

                        document.Add(new Paragraph($"Fecha de Corte: {data.FechaCorte}")
                            .SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));

                        document.Add(new Paragraph("\n")); 

                        Table table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 2, 2, 2, 2 })).UseAllAvailableWidth();

                        table.AddHeaderCell(new Cell().Add(new Paragraph("Organización Política").SetFont(bold).SetFontColor(ColorConstants.WHITE)))
                             .SetBackgroundColor(ColorConstants.DARK_GRAY);
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Siglas").SetFont(bold).SetFontColor(ColorConstants.WHITE)))
                             .SetBackgroundColor(ColorConstants.DARK_GRAY);
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Votos").SetFont(bold).SetFontColor(ColorConstants.WHITE)).SetTextAlignment(TextAlignment.CENTER))
                             .SetBackgroundColor(ColorConstants.DARK_GRAY);
                        table.AddHeaderCell(new Cell().Add(new Paragraph("%").SetFont(bold).SetFontColor(ColorConstants.WHITE)).SetTextAlignment(TextAlignment.CENTER))
                             .SetBackgroundColor(ColorConstants.DARK_GRAY);
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Escaños").SetFont(bold).SetFontColor(ColorConstants.WHITE)).SetTextAlignment(TextAlignment.CENTER))
                             .SetBackgroundColor(ColorConstants.DARK_GRAY);

                        foreach (var item in data.Resultados)
                        {
                            table.AddCell(new Paragraph(item.Lista).SetFont(normal));
                            table.AddCell(new Paragraph(item.Siglas).SetFont(normal));
                            table.AddCell(new Paragraph(item.VotosTotales.ToString("N0")).SetFont(normal).SetTextAlignment(TextAlignment.CENTER));
                            table.AddCell(new Paragraph(item.Porcentaje + "%").SetFont(normal).SetTextAlignment(TextAlignment.CENTER));

                            var celdaEscanos = new Cell().Add(new Paragraph(item.EscanosAsignados.ToString()).SetFont(bold).SetTextAlignment(TextAlignment.CENTER));
                            if (item.EscanosAsignados > 0) celdaEscanos.SetBackgroundColor(ColorConstants.LIGHT_GRAY);

                            table.AddCell(celdaEscanos);
                        }

                        document.Add(table);

                        document.Add(new Paragraph("\n"));
                        document.Add(new Paragraph($"Total Votos Válidos: {data.TotalVotos.ToString("N0")}")
                            .SetFont(bold));
                        document.Add(new Paragraph($"Escaños Repartidos: {data.TotalEscanos} (Método Webster)")
                            .SetFont(normal).SetFontSize(10));

                        document.Close();

                        return File(stream.ToArray(), "application/pdf", $"Resultados_{eleccionId}.pdf");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error interno creando PDF: {ex.Message}");
                }
            }

            return BadRequest($"No se pudieron obtener los datos. Mensaje: {apiResult?.Message ?? "Datos nulos"}");
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

        // GET: api/Votos/GUID
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Voto>>> GetVoto(Guid id)
        {
            try
            {
                var voto = await _context.Votos.Include(e => e.Eleccion).FirstOrDefaultAsync(e => e.Id == id);
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

        [HttpPost("emitir")]
        public async Task<ActionResult<ApiResult<string>>> EmitirVoto([FromBody] VotoRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var registroPadron = await _context.PadronElectorales
                    .Include(p => p.Votante).Include(p => p.Eleccion)
                    .FirstOrDefaultAsync(p => p.CodigoEnlace == request.CodigoEnlace && p.EleccionId == request.EleccionId);

                if (registroPadron == null) return ApiResult<string>.Fail("Código inválido.");
                if (registroPadron.CodigoCanjeado) return ApiResult<string>.Fail("Código ya usado.");

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

                try
                {
                    if (registroPadron.Votante != null && !string.IsNullOrEmpty(registroPadron.Votante.Correo))
                    {
                        EnviarCertificadoPDF(registroPadron.Votante.NombreCompleto ?? "Ciudadano", registroPadron.Votante.Correo, registroPadron.Eleccion?.Nombre ?? "Elección", DateTime.Now);
                    }
                }
                catch (Exception ex) { Log.Error($"Error email: {ex.Message}"); }

                return ApiResult<string>.Ok("Voto registrado.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResult<string>.Fail("Error interno.");
            }
        }

        private void EnviarCertificadoPDF(string nombreVotante, string correoDestino, string nombreEleccion, DateTime fecha)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);
                PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont normal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                document.Add(new Paragraph("CERTIFICADO DE VOTACIÓN").SetFont(bold).SetFontSize(20));
                document.Add(new Paragraph($"Ciudadano: {nombreVotante}").SetFont(normal));
                document.Add(new Paragraph($"Elección: {nombreEleccion}").SetFont(normal));
                document.Add(new Paragraph($"Fecha: {fecha}").SetFont(normal));
                document.Close();

                string miCorreo = "bastidaspaul83@gmail.com";
                string miPassword = "njkb gyyh qygc wviw";

                var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(miCorreo, miPassword),
                    EnableSsl = true,
                };
                var mail = new MailMessage { From = new MailAddress(miCorreo), Subject = "Certificado", Body = "Adjunto certificado." };
                mail.To.Add(correoDestino);
                mail.Attachments.Add(new Attachment(new MemoryStream(stream.ToArray()), "Certificado.pdf"));
                smtp.Send(mail);
            }
        }
    }
}