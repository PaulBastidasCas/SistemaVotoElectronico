using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class ListaElectoralesController : Controller
    {
        // GET: ListaElectoralesController
        public async Task<IActionResult> Index()
        {
            var res = await Crud<ListaElectoral>.ReadAllAsync();
            return View(res.Data ?? new List<ListaElectoral>());
        }

        // GET: ListaElectoralesController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<ListaElectoral>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // GET: ListaElectoralesController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ListaElectoralesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListaElectoral model)
        {
            if(!ModelState.IsValid) return View(model);

            var res = await Crud<ListaElectoral>.CreateAsync(model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model); 
        }

        // GET: ListaElectoralesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<ListaElectoral>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: ListaElectoralesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ListaElectoral model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<ListaElectoral>.UpdateAsync(id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: ListaElectoralesController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<ListaElectoral>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: ListaElectoralesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirned(int id)
        {
            await Crud<Candidato>.DeleteAsync(id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}
