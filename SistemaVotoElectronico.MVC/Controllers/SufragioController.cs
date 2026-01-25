using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;
using SistemaVotoElectronico.Modelos.Entidades;
using SistemaVotoElectronico.Modelos.Responses;
using SistemaVotoElectronico.MVC.Models;
using System.Text;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class SufragioController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBase;

        public SufragioController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _apiBase = configuration["ApiBaseUrl"] ?? "http://localhost:5051/api";
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidarCodigo(string codigoEnlace)
        {
            if (string.IsNullOrWhiteSpace(codigoEnlace))
            {
                ViewBag.Error = "Debe ingresar el código que le entregó el Jefe de Mesa.";
                return View("Index");
            }

            try
            {
                var eleccionesRes = await Crud<Eleccion>.ReadAllAsync($"{_apiBase}/Elecciones");

                if (!eleccionesRes.Success) throw new Exception("Error al conectar con el sistema de elecciones.");

                var eleccionActiva = eleccionesRes.Data?.FirstOrDefault(e => e.Activa);

                if (eleccionActiva == null)
                {
                    ViewBag.Error = "No hay ninguna elección activa en este momento.";
                    return View("Index");
                }
                var listasRes = await Crud<ListaElectoral>.ReadAllAsync($"{_apiBase}/ListaElectorales");

                var listasFiltradas = listasRes.Data?
                    .Where(l => l.EleccionId == eleccionActiva.Id)
                    .ToList() ?? new List<ListaElectoral>();

                var viewModel = new PapeletaViewModel
                {
                    Eleccion = eleccionActiva,
                    Listas = listasFiltradas,
                    CodigoEnlace = codigoEnlace.Trim().ToUpper()
                };

                return View("Papeleta", viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error del sistema: " + ex.Message;
                return View("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EmitirVoto(int eleccionId, string codigoEnlace, string seleccion)
        {
            if (string.IsNullOrEmpty(seleccion))
            {
                ViewBag.Error = "Su voto es nulo o inválido. Debe seleccionar una opción.";
                return View("ErrorVoto");
            }

            int? idLista = null;
            int? idCandidato = null;

            string[] partes = seleccion.Split('-');
            string tipoVoto = partes[0];
            int idSeleccionado = int.Parse(partes[1]);

            try
            {
                if (tipoVoto == "L")
                {
                    idLista = idSeleccionado;

                    var listaInfo = await Crud<ListaElectoral>.ReadByAsync($"{_apiBase}/ListaElectorales", "Id", idLista.ToString());

                    if (listaInfo.Success && listaInfo.Data != null && listaInfo.Data.Candidatos != null)
                    {
                        var candidatoPrincipal = listaInfo.Data.Candidatos
                            .FirstOrDefault(c => c.OrdenEnLista == 1);

                        if (candidatoPrincipal != null)
                        {
                            idCandidato = candidatoPrincipal.Id;
                        }
                    }
                }
                else if (tipoVoto == "C")
                {
                    idCandidato = idSeleccionado;
                }

                var votoRequest = new VotoRequest
                {
                    EleccionId = eleccionId,
                    CodigoEnlace = codigoEnlace,
                    IdListaSeleccionada = idLista,
                    IdCandidatoSeleccionado = idCandidato
                };

                var json = JsonConvert.SerializeObject(votoRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                string endpointSeguro = $"{_apiBase}/Votos/emitir";

                var response = await _httpClient.PostAsync(endpointSeguro, content);
                var responseString = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<ApiResult<string>>(responseString);

                if (response.IsSuccessStatusCode && (apiResult?.Success ?? false))
                {
                    return RedirectToAction("Confirmacion");
                }
                else
                {
                    ViewBag.Error = apiResult?.Message ?? "No se pudo registrar el voto. Intente nuevamente.";
                    return View("ErrorVoto");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error crítico de comunicación: " + ex.Message;
                return View("ErrorVoto");
            }
        }

        [HttpGet]
        public IActionResult Confirmacion()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return View();
        }

        [HttpGet]
        public IActionResult ErrorVoto()
        {
            return View();
        }
    }
}