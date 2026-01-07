using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class EleccionesController : Controller
    {
        // GET: EleccionesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: EleccionesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EleccionesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EleccionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EleccionesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EleccionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EleccionesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EleccionesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
