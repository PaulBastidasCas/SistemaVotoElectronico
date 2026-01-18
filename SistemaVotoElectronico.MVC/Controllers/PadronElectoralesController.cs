using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class PadronElectoralesController : Controller
    {
        // GET: PadronElectoralesController
        public async Task<IActionResult> Index()
        {
            var res = await Crud<PadronElectoral>.ReadAllAsync();
            return View(res.Data ?? new List<PadronElectoral>());
        }

        // GET: PadronElectoralesController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<PadronElectoral>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // GET: PadronElectoralesController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PadronElectoralesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PadronElectoral model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<PadronElectoral>.CreateAsync(model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: PadronElectoralesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<PadronElectoral>.ReadByAsync("Id", id.ToString());
            return View(res.Data); ;
        }

        // POST: PadronElectoralesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PadronElectoral model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<PadronElectoral>.UpdateAsync(id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: PadronElectoralesController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<PadronElectoral>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: PadronElectoralesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<PadronElectoral>.DeleteAsync(id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}
