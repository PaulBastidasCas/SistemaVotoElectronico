using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class VotosController : Controller
    {
        // GET: VotosController
        public async Task<IActionResult> Index()
        {
            var res = await Crud<Voto>.ReadAllAsync();
            return View(res.Data ?? new List<Voto>());
        }

        // GET: VotosController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<Voto>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // GET: VotosController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VotosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VotoRequest model) 
        {
            if (!ModelState.IsValid) return View(model);

            Crud<VotoRequest>.UrlBase = "https://localhost:7178/api/Votos";

            var res = await Crud<VotoRequest>.CreateAsync(model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: VotosController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<Voto>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: VotosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Voto model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Voto>.UpdateAsync(id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: VotosController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<Voto>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: VotosController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<Voto>.DeleteAsync(id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}
