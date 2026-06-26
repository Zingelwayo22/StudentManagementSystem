using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EnrollmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var enrollments = db.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Module)
                .ToList();

            return View(enrollments);
        }
        public ActionResult Edit(int id)
        {
            var enrollment = db.Enrollments.Find(id);

            if (enrollment == null)
            {
                return HttpNotFound();
            }

            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "FirstName", enrollment.StudentId);
            ViewBag.ModuleId = new SelectList(db.Modules, "ModuleId", "ModuleName", enrollment.ModuleId);

            return View(enrollment);
        }
        public ActionResult Delete(int id)
        {
            var enrollment = db.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Module)
                .FirstOrDefault(e => e.EnrollmentId == id);

            if (enrollment == null)
            {
                return HttpNotFound();
            }

            return View(enrollment);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var enrollment = db.Enrollments.Find(id);

            if (enrollment == null)
            {
                return HttpNotFound();
            }

            db.Enrollments.Remove(enrollment);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                // Prevent duplicate enrollment
                bool exists = db.Enrollments.Any(e =>
                    e.EnrollmentId != enrollment.EnrollmentId &&
                    e.StudentId == enrollment.StudentId &&
                    e.ModuleId == enrollment.ModuleId);

                if (exists)
                {
                    ModelState.AddModelError("", "This student is already enrolled in this module.");

                    ViewBag.StudentId = new SelectList(db.Students, "StudentId", "FirstName", enrollment.StudentId);
                    ViewBag.ModuleId = new SelectList(db.Modules, "ModuleId", "ModuleName", enrollment.ModuleId);

                    return View(enrollment);
                }

                db.Entry(enrollment).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(enrollment);
        }

        public ActionResult Create()
        {
            ViewBag.StudentId = new SelectList(
                db.Students,
                "StudentId",
                "FirstName");

            ViewBag.ModuleId = new SelectList(
                db.Modules,
                "ModuleId",
                "ModuleName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                db.Enrollments.Add(enrollment);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.StudentId = new SelectList(
                db.Students,
                "StudentId",
                "FirstName",
                enrollment.StudentId);

            ViewBag.ModuleId = new SelectList(
                db.Modules,
                "ModuleId",
                "ModuleName",
                enrollment.ModuleId);

            return View(enrollment);
        }
    }
}