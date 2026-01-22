using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class CandidatosController : Controller
    {
        private readonly string _endpoint = "http://localhost:5051/api/Candidatos";

        public async Task<ActionResult> Index()
        {
            var res = await Crud<Candidato>.ReadAllAsync(_endpoint);
            return View(res.Data ?? new List<Candidato>());
        }

        public async Task<ActionResult> Details(int id)
        {
            var res = await Crud<Candidato>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Candidato candidato)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    candidato.Id = 0;
                    candidato.ListaElectoral = null;

                    var resultado = await Crud<Candidato>.CreateAsync(_endpoint, candidato);

                    if (resultado.Success)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", $"Error API: {resultado.Message}");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error de conexión: {ex.Message}");
                }
            }
            return View(candidato);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<Candidato>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Candidato model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Candidato>.UpdateAsync(_endpoint, id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<Candidato>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<Candidato>.DeleteAsync(_endpoint, id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}