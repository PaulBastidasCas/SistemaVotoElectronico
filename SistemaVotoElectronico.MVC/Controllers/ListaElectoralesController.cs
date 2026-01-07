using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class ListaElectoralesController : Controller
    {
        // GET: ListaElectoralesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ListaElectoralesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ListaElectoralesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ListaElectoralesController/Create
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

        // GET: ListaElectoralesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ListaElectoralesController/Edit/5
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

        // GET: ListaElectoralesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ListaElectoralesController/Delete/5
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
