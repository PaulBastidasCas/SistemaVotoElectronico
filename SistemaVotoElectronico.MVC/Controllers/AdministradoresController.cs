using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class AdministradoresController : Controller
    {
        private readonly string _endpoint = "http://localhost:5051/api/Administradores";

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
        public async Task<IActionResult> Create(Administrador model, IFormFile? fotoUpload)
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

                    var resultado = await Crud<Administrador>.CreateAsync(_endpoint, model);

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

        // GET: AdministradoresController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<Administrador>.ReadByAsync(_endpoint, "Id", id.ToString());
            return View(res.Data);
        }

        // POST: AdministradoresController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Administrador model, IFormFile? fotoUpload)
        {
            var dbResult = await Crud<Administrador>.ReadByAsync(_endpoint, "Id", id.ToString());
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