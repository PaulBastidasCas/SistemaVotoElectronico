using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class VotantesController : Controller
    {
        // GET: VotantesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: VotantesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: VotantesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VotantesController/Create
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

        // GET: VotantesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: VotantesController/Edit/5
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

        // GET: VotantesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VotantesController/Delete/5
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
