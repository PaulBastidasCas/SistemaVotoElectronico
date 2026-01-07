using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class VotosController : Controller
    {
        // GET: VotosController
        public ActionResult Index()
        {
            return View();
        }

        // GET: VotosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: VotosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VotosController/Create
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

        // GET: VotosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: VotosController/Edit/5
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

        // GET: VotosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VotosController/Delete/5
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
