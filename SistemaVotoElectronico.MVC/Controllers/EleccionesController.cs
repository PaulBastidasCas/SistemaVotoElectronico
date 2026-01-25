using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos.Entidades;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class EleccionesController : Controller
    {
        private readonly string _endpoint;

        public EleccionesController(IConfiguration configuration)
        {
            string apiBase = configuration["ApiBaseUrl"] ?? "http://localhost:5051/api";
            _endpoint = $"{apiBase}/Elecciones";
        }

        public async Task<IActionResult> Index()
        {
            var res = await Crud<Eleccion>.ReadAllAsync(_endpoint);
            return View(res.Data ?? new List<Eleccion>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<Eleccion>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Eleccion model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Eleccion>.CreateAsync(_endpoint, model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<Eleccion>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Eleccion model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Eleccion>.UpdateAsync(_endpoint, id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<Eleccion>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var res = await Crud<Eleccion>.DeleteAsync(_endpoint, id.ToString());

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", $"No se pudo eliminar: {res.Message}");
            var reload = await Crud<Eleccion>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(reload.Data);
        }
    }
}