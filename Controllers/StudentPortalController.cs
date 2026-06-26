using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using StudentManagementSystem.Models;
using Microsoft.AspNet.Identity;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentPortalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: StudentPortal
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var student = db.Students
                .FirstOrDefault(s => s.UserId == userId);

            if (student == null)
            {
                return HttpNotFound();
            }
            var enrollments = db.Enrollments
                .Include(e => e.Module)
                .Where(e => e.StudentId == student.StudentId)
                .ToList();

            var marks = db.Marks
                .Where(m => m.StudentId == student.StudentId)
                .ToList();

            ViewBag.StudentName =
                student.FirstName + " " + student.LastName;

            ViewBag.ModuleCount = enrollments.Count();

            ViewBag.MarkCount = marks.Count();

            ViewBag.AverageScore =
                marks.Any()
                ? marks.Average(m => m.Score)
                : 0;
            double average =
    marks.Any()
    ? marks.Average(m => m.Score)
    : 0;

            ViewBag.AverageScore = average;

            if (average >= 75)
            {
                ViewBag.Status = "Excellent";
            }
            else if (average >= 50)
            {
                ViewBag.Status = "Passing";
            }
            else
            {
                ViewBag.Status = "At Risk";
            }

            return View(enrollments);
        }
        public ActionResult Results()
        {
            var userId = User.Identity.GetUserId();

            var student = db.Students
                .FirstOrDefault(s => s.UserId == userId);

            if (student == null)
            {
                return HttpNotFound();
            }

            var marks = db.Marks
                .Include(m => m.Module)
                .Where(m => m.StudentId == student.StudentId)
                .ToList();

            return View(marks);
        }

    }

}