using Budgeter.Models;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

        protected ApplicationUser GetUserInfo()
        {
            return db.Users.Find(User.Identity.GetUserId());
        }

        protected Household GetHouseholdInfo()
        {
            ApplicationUser user = GetUserInfo();
            return user == null ? null : db.Households.FirstOrDefault(h => h.Id == user.HouseholdId);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}