using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class MesasController : Controller
    {
        // GET: MesasController
        public async Task<IActionResult> Index()
        {
            var res = await Crud<Mesa>.ReadAllAsync();
            return View(res.Data ?? new List<Mesa>());
        }

        // GET: MesasController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<Mesa>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // GET: MesasController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MesasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mesa model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Mesa>.CreateAsync(model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: MesasController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<Mesa>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: MesasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Mesa model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Mesa>.UpdateAsync(id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: MesasController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<Mesa>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: MesasController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<Mesa>.DeleteAsync(id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}
