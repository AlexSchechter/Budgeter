using Budgeter.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    public class AppController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

        protected ApplicationUser UserInfo()
        {
            return db.Users.Find(User.Identity.GetUserId());
        }
        protected Household HouseholdInfo()
        {
            ApplicationUser user = UserInfo();
            if (user == null)
                return null;
            return (db.Households.FirstOrDefault(h => h.Id == user.HouseholdId));
        }
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