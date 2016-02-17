using Budgeter.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return View();
            return RedirectToAction("Login", "Account");
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

        //GET /Home/UserProfile
        public ActionResult UserProfile()
        {
            string userId = User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser user = db.Users.FirstOrDefault(u => u.Id == userId);
            ProfileViewModel model = new ProfileViewModel { Email = user.Email, FirstName = user.FirstName, LastName = user.LastName,
                Username = user.UserName, HouseholdName = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId).Name };

            return View(model);
        }



        //Get /Home/Households
        public  ActionResult Households()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string userId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.FirstOrDefault(u => u.Id == userId);
            int currentHouseholdId = db.Users.FirstOrDefault(u => u.Id == userId).HouseholdId;
            Household currentHousehold = db.Households.
                FirstOrDefault(h => h.Id == db.Users.FirstOrDefault(u => u.Id == userId).HouseholdId);
            List<int> householdOptionsIds = db.Invitations.Where(i => i.Email == user.Email).
                Where(i => i.HouseholdId != currentHouseholdId).Select(i => i.HouseholdId).ToList();
            List<Household> householdOptions= new List<Household>();
            foreach (int optionId in householdOptionsIds)
                householdOptions.Add(db.Households.FirstOrDefault(i => i.Id == optionId));          
            return View(new HouseholdViewModel { CurrentHousehold = currentHousehold, HouseholdOptions = householdOptions}); 
        }

    }
}