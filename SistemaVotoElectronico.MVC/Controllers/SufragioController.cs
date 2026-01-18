using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;
using SistemaVotoElectronico.MVC.Models;
using System.Text;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class SufragioController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBase = "http://localhost:5050/api"; 

        public SufragioController()
        {
            _httpClient = new HttpClient();
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
                var eleccionesRes = await Crud<Eleccion>.ReadAllAsync();
                if (!eleccionesRes.Success) throw new Exception("Error al conectar con el sistema de elecciones.");

                var eleccionActiva = eleccionesRes.Data?.FirstOrDefault(e => e.Activa);

                if (eleccionActiva == null)
                {
                    ViewBag.Error = "No hay ninguna elección activa en este momento.";
                    return View("Index");
                }

                var listasRes = await Crud<ListaElectoral>.ReadAllAsync();

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
            int idSeleccionado = int.Parse(partes[1]);

            if (partes[0] == "L") idLista = idSeleccionado;
            else if (partes[0] == "C") idCandidato = idSeleccionado;

            var votoRequest = new VotoRequest
            {
                EleccionId = eleccionId,
                CodigoEnlace = codigoEnlace,
                IdListaSeleccionada = idLista,
                IdCandidatoSeleccionado = idCandidato
            };

            try
            {
                var json = JsonConvert.SerializeObject(votoRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBase}/Votos", content);
                var responseString = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<ApiResult<object>>(responseString);

                if (response.IsSuccessStatusCode && apiResult.Success)
                {
                    return RedirectToAction("Confirmacion");
                }
                else
                {
                    ViewBag.Error = apiResult?.Message ?? "No se pudo registrar el voto.";
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
            return View();
        }

        [HttpGet]
        public IActionResult ErrorVoto()
        {
            return View();
        }
    }
}
