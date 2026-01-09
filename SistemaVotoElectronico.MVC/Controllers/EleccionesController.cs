using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class EleccionesController : Controller
    {
        // GET: EleccionesController
        public async Task<IActionResult> Index()
        {
            var res = await Crud<Eleccion>.ReadAllAsync();
            return View(res.Data ?? new List<Eleccion>());
        }

        // GET: EleccionesController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<Eleccion>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // GET: EleccionesController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EleccionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Eleccion model)
        {
            if(!ModelState.IsValid) return View(ModelState);

            var res = await Crud<Eleccion>.CreateAsync(model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: EleccionesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<Eleccion>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: EleccionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Eleccion model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Eleccion>.UpdateAsync(id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: EleccionesController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<Candidato>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: EleccionesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<Candidato>.DeleteAsync(id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}
