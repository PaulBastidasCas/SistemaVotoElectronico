using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class JefesDeMesaController : Controller
    {
        private readonly string _endpoint = "http://localhost:5051/api/JefesDeMesa";

        public async Task<IActionResult> Index()
        {
            var res = await Crud<JefeDeMesa>.ReadAllAsync(_endpoint);
            return View(res.Data ?? new List<JefeDeMesa>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<JefeDeMesa>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JefeDeMesa model, IFormFile? fotoUpload)
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

            if (string.IsNullOrEmpty(model.Contrasena))
            {
                ModelState.AddModelError("Contrasena", "La contraseña es obligatoria al crear un usuario.");
            }

            ModelState.Remove("Fotografia");

            if (ModelState.IsValid)
            {
                try
                {
                    model.Id = 0;
                    model.MesaAsignada = null;

                    var resultado = await Crud<JefeDeMesa>.CreateAsync(_endpoint, model);

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
            var res = await Crud<JefeDeMesa>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JefeDeMesa model, IFormFile? fotoUpload)
        {

            var dbResult = await Crud<JefeDeMesa>.ReadByAsync(_endpoint, "Id", id.ToString());
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

            if (original != null && model.Contrasena == original.Contrasena)
            {
                model.Contrasena = "";
            }

            ModelState.Remove("Contrasena");
            ModelState.Remove("Fotografia");

            if (!ModelState.IsValid) return View(model);

            var res = await Crud<JefeDeMesa>.UpdateAsync(_endpoint, id.ToString(), model);

            if (res.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<JefeDeMesa>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<JefeDeMesa>.DeleteAsync(_endpoint, id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}