using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos.DTOs;
using SistemaVotoElectronico.Modelos.Entidades;
using SistemaVotoElectronico.Modelos.Responses;
using System.Net.Http;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class VotosController : Controller
    {
        private readonly string _endpointVotos;
        private readonly HttpClient _httpClient;

        public VotosController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            string baseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5051/api";
            _endpointVotos = $"{baseUrl}/Votos";
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> DescargarPdf(int id)
        {
            var urlApi = $"{_endpointVotos}/reporte-pdf/{id}";

            var response = await _httpClient.GetAsync(urlApi);

            if (response.IsSuccessStatusCode)
            {
                var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                return File(pdfBytes, "application/pdf", $"Reporte_Eleccion_{id}.pdf");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Content($"Error desde la API: {response.StatusCode} - {errorContent}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Resultados(int id)
        {
            var urlApi = $"{_endpointVotos}/resultados/{id}?escanosA_Repartir=5";

            var response = await _httpClient.GetAsync(urlApi);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResult<ResultadoEleccionDto>>(json);

                if (result.Success) return View(result.Data);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var res = await Crud<Voto>.ReadAllAsync(_endpointVotos);
            return View(res.Data ?? new List<Voto>());
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var res = await Crud<Voto>.ReadByAsync(_endpointVotos, "Id", id.ToString());
            return View(res.Data);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Voto model)
        {
            if (model.FechaRegistro == default) model.FechaRegistro = DateTime.Now;
            if (model.Id == Guid.Empty) model.Id = Guid.NewGuid();
            ModelState.Remove("Eleccion");

            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Voto>.CreateAsync(_endpointVotos, model);
            if (res.Success) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var res = await Crud<Voto>.ReadByAsync(_endpointVotos, "Id", id.ToString());
            return View(res.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Voto model)
        {
            if (!ModelState.IsValid) return View(model);
            var res = await Crud<Voto>.UpdateAsync(_endpointVotos, id.ToString(), model);
            if (res.Success) return RedirectToAction(nameof(Index));
            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await Crud<Voto>.ReadByAsync(_endpointVotos, "Id", id.ToString());
            return View(res.Data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await Crud<Voto>.DeleteAsync(_endpointVotos, id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}