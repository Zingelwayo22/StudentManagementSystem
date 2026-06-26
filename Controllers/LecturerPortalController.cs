using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class LecturerPortalController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}