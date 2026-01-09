using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class AdministradoresController : Controller
    {
        // GET: AdministradoresController
        public async Task<IActionResult> Index()
        {
            var res = await Crud<Administrador>.ReadAllAsync();
            return View(res.Data ?? new List<Administrador>());
        }

        // GET: AdministradoresController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<Administrador>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // GET: AdministradoresController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdministradoresController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Administrador model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Administrador>.CreateAsync(model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: AdministradoresController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<Administrador>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: AdministradoresController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Administrador model)
        {
            // 1. Validar campos locales (Requeridos, formatos, etc.)
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Administrador>.UpdateAsync(id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: AdministradoresController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<Administrador>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: AdministradoresController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<Administrador>.DeleteAsync(id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}
