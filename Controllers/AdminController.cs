using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.StudentCount = db.Students.Count();
            ViewBag.LecturerCount = db.Lecturers.Count();
            ViewBag.ModuleCount = db.Modules.Count();
            ViewBag.EnrollmentCount = db.Enrollments.Count();
            ViewBag.MarkCount = db.Marks.Count();

            return View();
        }
    }
}