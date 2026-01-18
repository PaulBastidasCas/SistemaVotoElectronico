using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class CandidatosController : Controller
    {
        // GET: CandidatosController
        public async Task<ActionResult> Index()
        {
            var res = await Crud<Candidato>.ReadAllAsync();
            return View(res.Data ?? new List<Candidato>());
        }

        // GET: CandidatosController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var res = await Crud<Candidato>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // GET: CandidatosController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CandidatosController/Create
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

                    var resultado = await Crud<Candidato>.CreateAsync(candidato);

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
        // GET: CandidatosController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<Candidato>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        } 

        // POST: CandidatosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Candidato model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await Crud<Candidato>.UpdateAsync(id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        // GET: CandidatosController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<Candidato>.ReadByAsync("Id", id.ToString());
            return View(res.Data);
        }

        // POST: CandidatosController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<Candidato>.DeleteAsync(id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}
