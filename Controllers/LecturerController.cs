using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using StudentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LecturerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View(db.Lecturers.ToList());
        }
        public ActionResult Edit(int id)
        {
            var lecturer = db.Lecturers.Find(id);

            if (lecturer == null)
            {
                return HttpNotFound();
            }

            return View(lecturer);
        }
        public ActionResult Delete(int id)
        {
            var lecturer = db.Lecturers.Find(id);

            if (lecturer == null)
            {
                return HttpNotFound();
            }

            return View(lecturer);
        }
        public ActionResult Dashboard()
        {
            string userId = User.Identity.GetUserId();

            var lecturer = db.Lecturers
                .FirstOrDefault(l => l.UserId == userId);

            if (lecturer == null)
                return HttpNotFound();

            var modules = db.Modules
                .Where(m => m.LecturerId == lecturer.LecturerId)
                .ToList();

            var moduleIds = modules.Select(m => m.ModuleId).ToList();

            var enrollments = db.Enrollments
                .Where(e => moduleIds.Contains(e.ModuleId))
                .ToList();

            var marks = db.Marks
                .Where(m => moduleIds.Contains(m.ModuleId))
                .ToList();

            // METRICS
            ViewBag.ModuleCount = modules.Count;
            ViewBag.MarksCount = marks.Count;

            ViewBag.AverageScore = marks.Any()
                ? marks.Average(m => m.Score)
                : 0;

            ViewBag.Status =
                ViewBag.AverageScore >= 75 ? "Excellent" :
                ViewBag.AverageScore >= 50 ? "Good" : "Needs Improvement";

            // MODULE LIST WITH STUDENT COUNT
            var moduleData = modules.Select(m => new
            {
                m.ModuleId,
                m.ModuleName,
                StudentCount = enrollments.Count(e => e.ModuleId == m.ModuleId)
            }).ToList();

            ViewBag.ModuleData = moduleData;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Lecturer lecturer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lecturer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(lecturer);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var lecturer = db.Lecturers.Find(id);

            bool hasModules = db.Modules.Any(m => m.LecturerId == id);

            if (hasModules)
            {
                TempData["Error"] =
                    "Cannot delete lecturer because they are assigned to one or more modules.";

                return RedirectToAction("Index");
            }

            db.Lecturers.Remove(lecturer);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Lecturer lecturer)
        {
            if (ModelState.IsValid)
            {
                var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(db));

                var user = new ApplicationUser
                {
                    UserName = lecturer.Email,
                    Email = lecturer.Email
                };

                var result = userManager.Create(user, lecturer.Password);

                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Lecturer");

                    lecturer.UserId = user.Id;

                    db.Lecturers.Add(lecturer);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(lecturer);
        }
    }
}