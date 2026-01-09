using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class VotantesController : Controller
    {
        // GET: VotantesController
        public async Task<IActionResult> Index()
        {
            var res = await Crud<Votante>.ReadAllAsync();
            return View(res.Data ?? new List<Votante>());
        }

        // GET: VotantesController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<Votante>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // GET: VotantesController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VotantesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Votante model)
        {
            if(!ModelState.IsValid) return View(model);

            var res = await Crud<Votante>.CreateAsync(model);

            if (res.Success) 
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: VotantesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<Votante>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: VotantesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Votante model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Votante>.UpdateAsync(id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: VotantesController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<Votante>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: VotantesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<Votante>.DeleteAsync(id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}
