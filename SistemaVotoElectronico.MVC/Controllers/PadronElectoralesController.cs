using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class PadronElectoralesController : Controller
    {
        // GET: PadronElectoralesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PadronElectoralesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PadronElectoralesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PadronElectoralesController/Create
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

        // GET: PadronElectoralesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PadronElectoralesController/Edit/5
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

        // GET: PadronElectoralesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PadronElectoralesController/Delete/5
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
