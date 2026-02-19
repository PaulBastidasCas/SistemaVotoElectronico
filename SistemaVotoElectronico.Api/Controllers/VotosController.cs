using Microsoft.AspNetCore.Mvc;
using Serilog;
using SistemaVotoElectronico.Modelos.DTOs;
using SistemaVotoElectronico.Modelos.Entidades;
using SistemaVotoElectronico.Modelos.Responses;

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
            Console.WriteLine($"[INICIO] Intentando emitir voto para código: {request.CodigoEnlace}");

            // 1. Abrir transacción en BD
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 2. Buscar código electoral en el padrón
                var registroPadron = await _context.PadronElectorales
                    .Include(p => p.Votante)
                    .Include(p => p.Eleccion)
                    .FirstOrDefaultAsync(p => p.CodigoEnlace == request.CodigoEnlace && p.EleccionId == request.EleccionId);

                // 3. Validar si existe y no fue canjeado antes
                if (registroPadron == null)
                {
                    Console.WriteLine("[ERROR] Código no encontrado en BD"); 
                    return ApiResult<string>.Fail("Código inválido.");
                }

                if (registroPadron.CodigoCanjeado)
                {
                    Console.WriteLine("[ERROR] Código ya fue canjeado"); 
                    return ApiResult<string>.Fail("Código ya usado.");
                }

                // 4. Crear instancia del voto (plancha o nominal)
                var nuevoVoto = new Voto
                {
                    Id = Guid.NewGuid(),
                    EleccionId = request.EleccionId,
                    IdListaSeleccionada = request.IdListaSeleccionada,
                    IdCandidatoSeleccionado = request.IdCandidatoSeleccionado,
                    FechaRegistro = DateTime.Now
                };

                // 5. Añadir voto y "quemar" (invalidar) el código en el padrón
                _context.Votos.Add(nuevoVoto);
                registroPadron.CodigoCanjeado = true;
                registroPadron.FechaVoto = DateTime.Now;
                registroPadron.VotoPlanchaRealizado = (request.IdListaSeleccionada != null);
                registroPadron.VotoNominalRealizado = (request.IdCandidatoSeleccionado != null);

                // 6. Confirmar cambios y Commitear transacción
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine("[EXITO] Voto guardado en BD. Preparando correo...");

                // 7. Lanzar tarea asíncrona de fondo (Fire-and-Forget) para enviar certificado vía Email
                if (registroPadron.Votante != null && !string.IsNullOrEmpty(registroPadron.Votante.Correo))
                {
                    Console.WriteLine($"[INFO] Enviando correo a: {registroPadron.Votante.Correo}"); 

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
                            Console.WriteLine("[ASYNC] Iniciando envío SMTP...");
                            await EnviarCertificadoPDF(datos.Nombre, datos.Correo, datos.Eleccion, datos.Fecha);
                            Console.WriteLine("[ASYNC] ¡Correo enviado con éxito!"); 
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ASYNC ERROR] Falló el envío: {ex.Message}");
                            if (ex.InnerException != null) Console.WriteLine($"[INNER EX] {ex.InnerException.Message}");
                        }
                    });
                }
                else
                {
                    Console.WriteLine("[AVISO] El votante no tiene correo o es nulo. No se envía nada.");
                }

                return ApiResult<string>.Ok("Voto registrado.");
            }
            catch (Exception ex)
            {
                // 8. Revertir transacción en caso de error
                await transaction.RollbackAsync();
                Console.WriteLine($"[CRITICAL] Excepción en transacción: {ex.Message}");
                return ApiResult<string>.Fail("Error interno.");
            }
        }

        private async Task EnviarCertificadoPDF(string nombreVotante, string correoDestino, string nombreEleccion, DateTime fecha)
        {
            // 1. Inicializar Stream de memoria y documento PDF de iText
            using (MemoryStream stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // 2. Configurar fuentes
                PdfFont fuenteNegrita = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont fuenteNormal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                // 3. Dibujar textos y variables en el PDF
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

                // 4. Convertir documento a array de bytes
                byte[] bytesPdf = stream.ToArray();

                string miCorreo = "bastidaspaul83@gmail.com";
                string miPassword = "njkb gyyh qygc wviw";

                // 5. Configurar cliente SMTP y enviar correo con el byte array como adjunto
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

        [HttpGet("resultados/{eleccionId}")]
        public async Task<ActionResult<ApiResult<ResultadoEleccionDto>>> GetResultados(int eleccionId, int escanosA_Repartir = 5)
        {
            try
            {
                // 1. Obtener la elección y los votos registrados para la misma
                var eleccion = await _context.Elecciones.FindAsync(eleccionId);
                if (eleccion == null) return ApiResult<ResultadoEleccionDto>.Fail("Elección no encontrada");

                var votos = await _context.Votos
                    .AsNoTracking()
                    .Where(v => v.EleccionId == eleccionId) 
                    .ToListAsync();

                var listas = await _context.ListaElectorales
                    .AsNoTracking()
                    .Include(l => l.Candidatos)
                    .Where(l => l.EleccionId == eleccionId)
                    .ToListAsync();

                // 2. Iterar listas y contabilizar votos (plancha o por candidato individual)
                var resultadosListas = new List<DetalleListaDto>();
                int totalVotosValidos = 0;

                foreach (var lista in listas)
                {
                    var idsCandidatos = lista.Candidatos.Select(c => c.Id).ToList();

                    int totalLista = votos.Count(v =>
                        v.IdListaSeleccionada == lista.Id ||
                        (v.IdCandidatoSeleccionado.HasValue && idsCandidatos.Contains(v.IdCandidatoSeleccionado.Value))
                    );

                    totalVotosValidos += totalLista;

                    resultadosListas.Add(new DetalleListaDto
                    {
                        Lista = lista.Nombre,
                        Siglas = lista.Siglas,
                        Color = !string.IsNullOrEmpty(lista.Color) ? lista.Color : "#6c757d",
                        VotosTotales = totalLista,
                        EscanosAsignados = 0
                    });
                }

                // 3. Calcular porcentajes totales
                if (totalVotosValidos > 0)
                {
                    resultadosListas.ForEach(r => r.Porcentaje = Math.Round(((double)r.VotosTotales / totalVotosValidos) * 100, 2));
                }

                // 4. Aplicar Método Webster (Cocientes impares 1, 3, 5...) para reparto de escaños
                var cocientes = new List<dynamic>();
                foreach (var item in resultadosListas)
                {
                    for (int i = 1; i <= escanosA_Repartir * 2; i += 2)
                    {
                        cocientes.Add(new { Lista = item, Valor = (double)item.VotosTotales / i });
                    }
                }

                // 5. Asignar escaños basados en los cocientes más altos
                var escanosGanadores = cocientes.OrderByDescending(x => x.Valor).Take(escanosA_Repartir).ToList();
                foreach (var ganador in escanosGanadores)
                {
                    DetalleListaDto listaGanadora = ganador.Lista;
                    listaGanadora.EscanosAsignados++;
                }

                // 6. Generar estructura DTO final
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
            // 1. Obtener los resultados matemáticos invocando al método GetResultados
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
                    // 2. Iniciar MemoryStream y crear PDF con iText
                    using (MemoryStream stream = new MemoryStream())
                    {
                        PdfWriter writer = new PdfWriter(stream);
                        PdfDocument pdf = new PdfDocument(writer);
                        Document document = new Document(pdf);

                        // 3. Establecer cabeceras del documento
                        PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                        PdfFont normal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                        document.Add(new Paragraph("REPORTE DE RESULTADOS OFICIALES")
                            .SetFont(bold).SetFontSize(18).SetTextAlignment(TextAlignment.CENTER));

                        document.Add(new Paragraph(data.Eleccion)
                            .SetFont(bold).SetFontSize(14).SetTextAlignment(TextAlignment.CENTER));

                        document.Add(new Paragraph($"Fecha de Corte: {data.FechaCorte}")
                            .SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));

                        document.Add(new Paragraph("\n"));

                        // 4. Crear estructura de Tabla para los resultados
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

                        // 5. Poblar tabla iterando los resultados
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

                        // 6. Renderizar tabla y pie de página en el documento
                        document.Add(table);

                        document.Add(new Paragraph("\n"));
                        document.Add(new Paragraph($"Total Votos Válidos: {data.TotalVotos.ToString("N0")}")
                            .SetFont(bold));
                        document.Add(new Paragraph($"Escaños Repartidos: {data.TotalEscanos} (Método Webster)")
                            .SetFont(normal).SetFontSize(10));

                        document.Close();

                        // 7. Retornar archivo PDF directamente como stream
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