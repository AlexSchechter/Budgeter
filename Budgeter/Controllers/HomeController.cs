using Budgeter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult UserProfile(string userId)
        {
            if (userId == null)
                RedirectToAction("Index");
            var db = new ApplicationDbContext();
            ApplicationUser user = db.Users.FirstOrDefault(u => u.Id == userId);
            ProfileViewModel model = new ProfileViewModel { Email = user.Email, FirstName = user.FirstName, LastName = user.LastName,
                Username = user.UserName, HouseholdName = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId).Name };

            return View(model);
        }
    }
}