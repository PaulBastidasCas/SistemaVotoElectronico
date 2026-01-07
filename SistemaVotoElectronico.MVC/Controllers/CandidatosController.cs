using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class CandidatosController : Controller
    {
        // GET: CandidatosController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CandidatosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CandidatosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CandidatosController/Create
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

        // GET: CandidatosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CandidatosController/Edit/5
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

        // GET: CandidatosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CandidatosController/Delete/5
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
