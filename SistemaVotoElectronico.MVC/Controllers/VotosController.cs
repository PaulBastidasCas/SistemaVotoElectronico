using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class VotosController : Controller
    {
        private readonly string _endpoint = "http://localhost:5050/api/Votos";

        public async Task<IActionResult> Index()
        {
            var res = await Crud<Voto>.ReadAllAsync(_endpoint);
            return View(res.Data ?? new List<Voto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<Voto>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Voto model) 
        {
            if (model.FechaRegistro == default)
            {
                model.FechaRegistro = DateTime.Now;
            }

            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }

            ModelState.Remove("Eleccion");

            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Voto>.CreateAsync(_endpoint, model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<Voto>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Voto model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Voto>.UpdateAsync(_endpoint, id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<Voto>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<Voto>.DeleteAsync(_endpoint, id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}