using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class ListaElectoralesController : Controller
    {
        private readonly string _endpoint = "http://localhost:5051/api/ListaElectorales";

        public async Task<IActionResult> Index()
        {
            var res = await Crud<ListaElectoral>.ReadAllAsync(_endpoint);
            return View(res.Data ?? new List<ListaElectoral>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<ListaElectoral>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListaElectoral model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<ListaElectoral>.CreateAsync(_endpoint, model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<ListaElectoral>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ListaElectoral model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<ListaElectoral>.UpdateAsync(_endpoint, id.ToString(), model);

            if (res.Success)
            {
                if (User.IsInRole("Candidato"))
                {
                    return RedirectToAction("Perfil", "Home");
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<ListaElectoral>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<ListaElectoral>.DeleteAsync(_endpoint, id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}