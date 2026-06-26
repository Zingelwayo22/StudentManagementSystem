using System.Linq;
using System.Web.Mvc;
using StudentManagementSystem.Models;
using System.Data.Entity;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ModuleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View(
    db.Modules
      .Include(m => m.Lecturer)
      .ToList());
        }
        public ActionResult Edit(int id)
        {
            var module = db.Modules.Find(id);

            if (module == null)
            {
                return HttpNotFound();
            }

            ViewBag.LecturerId = new SelectList(db.Lecturers, "LecturerId", "FirstName", module.LecturerId);

            return View(module);
        }
        public ActionResult Delete(int id)
        {
            var module = db.Modules.Find(id);

            if (module == null)
            {
                return HttpNotFound();
            }

            return View(module);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Module module)
        {
            if (ModelState.IsValid)
            {
                db.Entry(module).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.LecturerId = new SelectList(db.Lecturers, "LecturerId", "FirstName", module.LecturerId);
            return View(module);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var module = db.Modules.Find(id);

            db.Modules.Remove(module);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            ViewBag.LecturerId = new SelectList(
        db.Lecturers,
        "LecturerId",
        "FirstName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Module module)
        {
            if (ModelState.IsValid)
            {
                db.Modules.Add(module);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.LecturerId = new SelectList(
     db.Lecturers,
     "LecturerId",
     "FirstName",
     module.LecturerId);

            return View(module);
        }
    }
}