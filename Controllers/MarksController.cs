using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using StudentManagementSystem.Models;
using Microsoft.AspNet.Identity;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class MarksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // =========================
        // INDEX (VIEW OWN MARKS)
        // =========================
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();

            var lecturer = db.Lecturers
                .FirstOrDefault(l => l.UserId == userId);

            if (lecturer == null)
                return HttpNotFound();

            var marks = db.Marks
                .Include(m => m.Student)
                .Include(m => m.Module)
                .Where(m => m.Module.LecturerId == lecturer.LecturerId)
                .ToList();

            return View(marks);
        }
        public ActionResult Edit(int id)
        {
            string userId = User.Identity.GetUserId();

            var lecturer = db.Lecturers
                .FirstOrDefault(l => l.UserId == userId);

            var mark = db.Marks
                .Include(m => m.Module)
                .FirstOrDefault(m => m.MarkId == id);

            if (mark == null)
                return HttpNotFound();

            // SECURITY CHECK
            if (mark.Module.LecturerId != lecturer.LecturerId)
                return new HttpStatusCodeResult(403);

            return View(mark);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Mark mark)
        {
            string userId = User.Identity.GetUserId();

            var lecturer = db.Lecturers
                .FirstOrDefault(l => l.UserId == userId);

            var existingMark = db.Marks
                .Include(m => m.Module)
                .FirstOrDefault(m => m.MarkId == mark.MarkId);

            if (existingMark == null)
                return HttpNotFound();

            // SECURITY CHECK
            if (existingMark.Module.LecturerId != lecturer.LecturerId)
                return new HttpStatusCodeResult(403);

            if (ModelState.IsValid)
            {
                existingMark.Score = mark.Score;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(mark);
        }
        public ActionResult Delete(int id)
        {
            string userId = User.Identity.GetUserId();

            var lecturer = db.Lecturers
                .FirstOrDefault(l => l.UserId == userId);

            var mark = db.Marks
                .Include(m => m.Module)
                .Include(m => m.Student)
                .FirstOrDefault(m => m.MarkId == id);

            if (mark == null)
                return HttpNotFound();

            // SECURITY CHECK
            if (mark.Module.LecturerId != lecturer.LecturerId)
                return new HttpStatusCodeResult(403);

            return View(mark);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string userId = User.Identity.GetUserId();

            var lecturer = db.Lecturers
                .FirstOrDefault(l => l.UserId == userId);

            var mark = db.Marks
                .Include(m => m.Module)
                .FirstOrDefault(m => m.MarkId == id);

            if (mark == null)
                return HttpNotFound();

            // SECURITY CHECK
            if (mark.Module.LecturerId != lecturer.LecturerId)
                return new HttpStatusCodeResult(403);

            db.Marks.Remove(mark);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // =========================
        // CREATE (GET)
        // =========================
        public ActionResult Create()
        {
            string userId = User.Identity.GetUserId();

            var lecturer = db.Lecturers
                .FirstOrDefault(l => l.UserId == userId);

            if (lecturer == null)
                return HttpNotFound();

            var lecturerModules = db.Modules
                .Where(m => m.LecturerId == lecturer.LecturerId)
                .ToList();

            var moduleIds = lecturerModules.Select(m => m.ModuleId).ToList();

            var students = db.Enrollments
                .Where(e => moduleIds.Contains(e.ModuleId))
                .Select(e => e.Student)
                .Distinct()
                .ToList();

            ViewBag.StudentId = new SelectList(students, "StudentId", "FirstName");
            ViewBag.ModuleId = new SelectList(lecturerModules, "ModuleId", "ModuleName");

            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Mark mark)
        {
            if (!ModelState.IsValid)
                return View(mark);

            string userId = User.Identity.GetUserId();

            var lecturer = db.Lecturers
                .FirstOrDefault(l => l.UserId == userId);

            if (lecturer == null)
                return new HttpStatusCodeResult(403);

            // Check module belongs to lecturer
            bool validModule = db.Modules.Any(m =>
                m.ModuleId == mark.ModuleId &&
                m.LecturerId == lecturer.LecturerId);

            if (!validModule)
                return new HttpStatusCodeResult(403);

            // Check student enrolled in module
            bool validStudent = db.Enrollments.Any(e =>
                e.StudentId == mark.StudentId &&
                e.ModuleId == mark.ModuleId);

            if (!validStudent)
                return new HttpStatusCodeResult(403);

            // Check if mark exists (UPDATE)
            var existingMark = db.Marks.FirstOrDefault(m =>
                m.StudentId == mark.StudentId &&
                m.ModuleId == mark.ModuleId);

            if (existingMark != null)
            {
                existingMark.Score = mark.Score;
            }
            else
            {
                db.Marks.Add(mark);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // =========================
        // DISPOSE
        // =========================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}