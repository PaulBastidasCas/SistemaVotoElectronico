using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class AdministradoresController : Controller
    {
        private readonly string _endpoint = "http://localhost:5050/api/Administradores";

        // GET: AdministradoresController
        public async Task<IActionResult> Index()
        {
            var res = await Crud<Administrador>.ReadAllAsync(_endpoint);
            return View(res.Data ?? new List<Administrador>());
        }

        // GET: AdministradoresController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<Administrador>.ReadByAsync(_endpoint, "Id", id.ToString());
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

            var res = await Crud<Administrador>.CreateAsync(_endpoint, model);

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
            var res = await Crud<Administrador>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        // POST: AdministradoresController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Administrador model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Administrador>.UpdateAsync(_endpoint, id.ToString(), model);

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
            var res = await Crud<Administrador>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        // POST: AdministradoresController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<Administrador>.DeleteAsync(_endpoint, id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}