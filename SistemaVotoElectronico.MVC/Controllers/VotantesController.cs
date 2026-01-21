using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class VotantesController : Controller
    {
        private readonly string _endpoint = "http://localhost:5050/api/Votantes";

        public async Task<IActionResult> Index()
        {
            var res = await Crud<Votante>.ReadAllAsync(_endpoint);
            return View(res.Data ?? new List<Votante>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<Votante>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Votante model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Votante>.CreateAsync(_endpoint, model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<Votante>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Votante model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Votante>.UpdateAsync(_endpoint, id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<Votante>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<Votante>.DeleteAsync(_endpoint, id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}