using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos.Entidades;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class VicepresidentesController : Controller
    {
        private readonly string _endpoint;
        private readonly string _endpointListas;

        public VicepresidentesController(IConfiguration configuration)
        {
            string apiBase = configuration["ApiBaseUrl"] ?? "http://localhost:5051/api";
            _endpoint = $"{apiBase}/Vicepresidentes";
            _endpointListas = $"{apiBase}/ListaElectorales";
        }

        public async Task<IActionResult> Index()
        {
            var res = await Crud<Vicepresidente>.ReadAllAsync(_endpoint);
            return View(res.Data ?? new List<Vicepresidente>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<Vicepresidente>.ReadByAsync(_endpoint, "Id", id.ToString());
            if (!res.Success || res.Data == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(res.Data);
        }

        public async Task<IActionResult> Create()
        {
            var listasResult = await Crud<ListaElectoral>.ReadAllAsync(_endpointListas);
            ViewBag.Listas = listasResult.Data ?? new List<ListaElectoral>();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vicepresidente model, IFormFile? fotoUpload)
        {
            if (fotoUpload != null && fotoUpload.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await fotoUpload.CopyToAsync(ms);
                    var bytes = ms.ToArray();
                    var base64 = Convert.ToBase64String(bytes);
                    var ext = Path.GetExtension(fotoUpload.FileName).Replace(".", "");
                    model.Fotografia = $"data:image/{ext};base64,{base64}";
                }
            }
            else
            {
                model.Fotografia = "";
            }

            ModelState.Remove("Fotografia");
            ModelState.Remove("ListaElectoral");

            if (ModelState.IsValid)
            {
                try
                {
                    model.Id = 0;
                    var resultado = await Crud<Vicepresidente>.CreateAsync(_endpoint, model);

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
            var listasResult = await Crud<ListaElectoral>.ReadAllAsync(_endpointListas);
            ViewBag.Listas = listasResult.Data ?? new List<ListaElectoral>();
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<Vicepresidente>.ReadByAsync(_endpoint, "Id", id.ToString());
            if (!res.Success || res.Data == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var listasResult = await Crud<ListaElectoral>.ReadAllAsync(_endpointListas);
            ViewBag.Listas = listasResult.Data ?? new List<ListaElectoral>();
            return View(res.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vicepresidente model, IFormFile? fotoUpload)
        {
            var dbResult = await Crud<Vicepresidente>.ReadByAsync(_endpoint, "Id", id.ToString());
            var original = dbResult.Data; 

            if (fotoUpload != null && fotoUpload.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await fotoUpload.CopyToAsync(ms);
                    var bytes = ms.ToArray();
                    var base64 = Convert.ToBase64String(bytes);
                    var ext = Path.GetExtension(fotoUpload.FileName).Replace(".", "");
                    model.Fotografia = $"data:image/{ext};base64,{base64}";
                }
            }
            else
            {
                model.Fotografia = original?.Fotografia;
            }

            ModelState.Remove("Fotografia");
            ModelState.Remove("ListaElectoral");

            if (!ModelState.IsValid) 
            {
                var listasResult = await Crud<ListaElectoral>.ReadAllAsync(_endpointListas);
                ViewBag.Listas = listasResult.Data ?? new List<ListaElectoral>();
                return View(model);
            }

            var res = await Crud<Vicepresidente>.UpdateAsync(_endpoint, id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            var listasResult2 = await Crud<ListaElectoral>.ReadAllAsync(_endpointListas);
            ViewBag.Listas = listasResult2.Data ?? new List<ListaElectoral>();
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<Vicepresidente>.ReadByAsync(_endpoint, "Id", id.ToString());
            if (!res.Success || res.Data == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(res.Data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<Vicepresidente>.DeleteAsync(_endpoint, id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}
