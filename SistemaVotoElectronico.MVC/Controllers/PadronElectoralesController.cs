using Microsoft.AspNetCore.Mvc;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class PadronElectoralesController : Controller
    {
        private readonly string _baseApiUrl = "http://localhost:5051/api";

        [HttpGet]
        public IActionResult Autorizar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarCodigo(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
            {
                ViewBag.Error = "Ingrese el número de cédula.";
                return View("Autorizar");
            }

            var resultadoElecciones = await Crud<Eleccion>.ReadAllAsync($"{_baseApiUrl}/Elecciones");
            var eleccionActiva = resultadoElecciones.Data?.FirstOrDefault(e => e.Activa);

            if (eleccionActiva == null)
            {
                ViewBag.Error = "No hay elecciones activas configuradas en el sistema.";
                return View("Autorizar");
            }

            var correoJefe = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var resultadoJefes = await Crud<JefeDeMesa>.ReadAllAsync($"{_baseApiUrl}/JefesDeMesa");

            var jefeActual = resultadoJefes.Data?.FirstOrDefault(j =>
                j.Correo != null && j.Correo.Equals(correoJefe, StringComparison.OrdinalIgnoreCase));

            if (jefeActual == null || jefeActual.MesaAsignada == null)
            {
                ViewBag.Error = "Su usuario no tiene una mesa asignada para gestionar.";
                return View("Autorizar");
            }

            var request = new GenerarCodigoRequest
            {
                CedulaVotante = cedula.Trim(),
                EleccionId = eleccionActiva.Id,
                MesaId = jefeActual.MesaAsignada.Id
            };

            var resultadoCodigo = await Crud<object>.PostAndGetResultAsync<GenerarCodigoRequest, string>(
                $"{_baseApiUrl}/PadronElectorales/generar-codigo",
                request
            );

            if (resultadoCodigo.Success)
            {
                ViewBag.CodigoGenerado = resultadoCodigo.Data; 
                ViewBag.Cedula = cedula;
            }
            else
            {
                ViewBag.Error = resultadoCodigo.Message;
            }

            return View("Autorizar");
        }

        public async Task<IActionResult> Index()
        {
            var res = await Crud<PadronElectoral>.ReadAllAsync($"{_baseApiUrl}/PadronElectorales");
            return View(res.Data ?? new List<PadronElectoral>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var res = await Crud<PadronElectoral>.ReadByAsync($"{_baseApiUrl}/PadronElectorales", "Id", id.ToString());
            if (res.Data == null) return NotFound();
            return View(res.Data);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PadronElectoral padronElectoral)
        {
            ModelState.Remove("Votante");
            ModelState.Remove("Mesa");
            ModelState.Remove("Eleccion");

            if (ModelState.IsValid)
            {
                var res = await Crud<PadronElectoral>.CreateAsync($"{_baseApiUrl}/PadronElectorales", padronElectoral);
                if (res.Success) return RedirectToAction(nameof(Index));

                ModelState.AddModelError("", res.Message);
            }

            return View(padronElectoral);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var res = await Crud<PadronElectoral>.ReadByAsync($"{_baseApiUrl}/PadronElectorales", "Id", id.ToString());
            if (res.Data == null) return NotFound();

            return View(res.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PadronElectoral padronElectoral)
        {
            if (id != padronElectoral.Id) return NotFound();

            ModelState.Remove("Votante");
            ModelState.Remove("Mesa");
            ModelState.Remove("Eleccion");

            if (ModelState.IsValid)
            {
                var res = await Crud<PadronElectoral>.UpdateAsync($"{_baseApiUrl}/PadronElectorales", id.ToString(), padronElectoral);
                if (res.Success) return RedirectToAction(nameof(Index));

                ModelState.AddModelError("", res.Message);
            }

            return View(padronElectoral);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var res = await Crud<PadronElectoral>.ReadByAsync($"{_baseApiUrl}/PadronElectorales", "Id", id.ToString());
            if (res.Data == null) return NotFound();
            return View(res.Data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Crud<PadronElectoral>.DeleteAsync($"{_baseApiUrl}/PadronElectorales", id.ToString());
            return RedirectToAction(nameof(Index));
        }

    }
}