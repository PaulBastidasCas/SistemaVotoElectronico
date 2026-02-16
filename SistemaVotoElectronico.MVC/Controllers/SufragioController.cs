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
                ViewBag.Error = "Debe ingresar el codigo que le entrego el Jefe de Mesa.";
                return View("Index");
            }

            try
            {
                var eleccionesRes = await Crud<Eleccion>.ReadAllAsync($"{_apiBase}/Elecciones");

                if (!eleccionesRes.Success) throw new Exception("Error al conectar con el sistema de elecciones.");

                var eleccionActiva = eleccionesRes.Data?.FirstOrDefault(e => e.Activa);

                if (eleccionActiva == null)
                {
                    ViewBag.Error = "No hay ninguna eleccion activa en este momento.";
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
        public async Task<IActionResult> EmitirVoto(int eleccionId, string codigoEnlace, string seleccionPresidente, string seleccionAsambleista)
        {
            // Validacion basica
            if (string.IsNullOrEmpty(seleccionPresidente) || string.IsNullOrEmpty(seleccionAsambleista))
            {
                ViewBag.Error = "Debe realizar una seleccion en ambas papeletas (o votar NULO).";
                return View("ErrorVoto");
            }

            int? idListaPresidente = null;
            int? idListaAsambleista = null;

            // Procesar voto Presidente
            if (seleccionPresidente != "NULO")
            {
                if (int.TryParse(seleccionPresidente, out int idPres))
                {
                    idListaPresidente = idPres;
                }
            }

            // Procesar voto Asambleista
            if (seleccionAsambleista != "NULO")
            {
                if (int.TryParse(seleccionAsambleista, out int idAsam))
                {
                    idListaAsambleista = idAsam;
                }
            }

            try
            {
                var votoRequest = new VotoRequest
                {
                    EleccionId = eleccionId,
                    CodigoEnlace = codigoEnlace,
                    ListaPresidenteId = idListaPresidente,
                    ListaAsambleistaId = idListaAsambleista
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
                ViewBag.Error = "Error critico de comunicacion con el servidor: " + ex.Message;
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