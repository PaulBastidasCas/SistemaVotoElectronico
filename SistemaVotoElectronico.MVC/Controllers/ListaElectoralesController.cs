using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos.Entidades;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class ListaElectoralesController : Controller
    {
        private readonly string _endpoint;

        public ListaElectoralesController(IConfiguration configuration)
        {
            string apiBase = configuration["ApiBaseUrl"] ?? "http://localhost:5051/api";
            _endpoint = $"{apiBase}/ListaElectorales";
        }

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
        public async Task<IActionResult> Create(ListaElectoral model, IFormFile? fotoUpload)
        {
            if (fotoUpload != null && fotoUpload.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await fotoUpload.CopyToAsync(ms);
                    var bytes = ms.ToArray();
                    var base64 = Convert.ToBase64String(bytes);
                    var ext = Path.GetExtension(fotoUpload.FileName).Replace(".", "");
                    model.Logotipo = $"data:image/{ext};base64,{base64}";
                }
            }
            else
            {
                model.Logotipo = "";
            }

            ModelState.Remove("Logotipo");

            if (ModelState.IsValid)
            {
                try
                {
                    model.Id = 0;
                    model.Eleccion = null;
                    model.Candidatos = null;

                    var resultado = await Crud<ListaElectoral>.CreateAsync(_endpoint, model);

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
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<ListaElectoral>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ListaElectoral model, IFormFile? fotoUpload)
        {
            var dbResult = await Crud<ListaElectoral>.ReadByAsync(_endpoint, "Id", id.ToString());
            var original = dbResult.Data;

            if (fotoUpload != null && fotoUpload.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await fotoUpload.CopyToAsync(ms);
                    var bytes = ms.ToArray();
                    var base64 = Convert.ToBase64String(bytes);
                    var ext = Path.GetExtension(fotoUpload.FileName).Replace(".", "");
                    model.Logotipo = $"data:image/{ext};base64,{base64}";
                }
            }
            else
            {
                model.Logotipo = original?.Logotipo;
            }

            ModelState.Remove("Logotipo");

            if (!ModelState.IsValid) return View(model);

            var res = await Crud<ListaElectoral>.UpdateAsync(_endpoint, id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
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