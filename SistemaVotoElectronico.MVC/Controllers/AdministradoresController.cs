using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class AdministradoresController : Controller
    {
        // GET: AdministradoresController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AdministradoresController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdministradoresController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdministradoresController/Create
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

        // GET: AdministradoresController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdministradoresController/Edit/5
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

        // GET: AdministradoresController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdministradoresController/Delete/5
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
