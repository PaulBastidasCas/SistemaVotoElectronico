using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class JefesDeMesaController : Controller
    {
        // GET: JefesDeMesaController
        public async Task<IActionResult> Index()
        {
            var res = await Crud<JefeDeMesa>.ReadAllAsync();
            return View(res.Data ?? new List<JefeDeMesa>());
        }

        // GET: JefesDeMesaController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<JefeDeMesa>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // GET: JefesDeMesaController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JefesDeMesaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JefeDeMesa model)
        {

            if (!ModelState.IsValid) return View(model);

            var res = await Crud<JefeDeMesa>.CreateAsync(model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: JefesDeMesaController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<JefeDeMesa>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: JefesDeMesaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JefeDeMesa model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<JefeDeMesa>.UpdateAsync(id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: JefesDeMesaController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<JefeDeMesa>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: JefesDeMesaController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<JefeDeMesa>.DeleteAsync(id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}
