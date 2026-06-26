using StudentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Student
        public ActionResult Index()
        {
            return View(db.Students.ToList());
        }
        public ActionResult Edit(int id)
        {
            var student = db.Students.Find(id);

            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int StudentId, string FirstName, string LastName)
        {
            var student = db.Students.Find(StudentId);

            if (student == null)
                return HttpNotFound();

            student.FirstName = FirstName;
            student.LastName = LastName;

            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            var student = db.Students.Find(id);

            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var student = db.Students.Find(id);

            db.Students.Remove(student);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                var userManager =
                    new UserManager<ApplicationUser>(
                        new UserStore<ApplicationUser>(db));

                var user = new ApplicationUser
                {
                    UserName = student.Email,
                    Email = student.Email
                };

                var result =
                    userManager.Create(user, student.Password);

                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Student");

                    student.UserId = user.Id;

                    db.Students.Add(student);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(student);
        }
    }
}