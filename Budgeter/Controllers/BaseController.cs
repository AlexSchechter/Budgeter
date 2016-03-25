using Budgeter.Models;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    public class BaseController : Controller
    {
        protected readonly string DemoEmail = "john@snow.com";
        protected ApplicationDbContext db = new ApplicationDbContext();

        protected ApplicationUser GetUserInfo()
        {
            var u = User.Identity.Name;
            return db.Users.Find(User.Identity.GetUserId());
            
        }

        protected Household GetHouseholdInfo()
        {
            ApplicationUser user = GetUserInfo();
            return user == null ? null : db.Households.FirstOrDefault(h => h.Id == user.HouseholdId);
        }

        protected void PopulateCategories(int householdId)
        {
            Household household = db.Households.Find(householdId);
            string[] categoryNames = {"Automobile", "Bank Charges", "Charity", "Childcare", "Clothing", "Credit Card Fees", "Education",
                "Events", "Food", "Gifts", "Healthcare", "Household", "Savings Interest", "Insurance", "Job Expenses", "Leisure (not holiday)", "Hobbies",
                "Loans", "Misc", "Pet Care", "Salary", "Savings", "Taxes", "Utilities", "Holiday"};

            Category myCategory = new Category();
            foreach (string name in categoryNames)
            {
                if (!db.Categories.Any(c => c.Name == name))
                {
                    db.Categories.Add(new Category { Name = name });
                    db.SaveChanges();
                }

                myCategory = db.Categories.FirstOrDefault(c => c.Name == name);
                household.Categories.Add(myCategory);              
            }
            db.SaveChanges();
        } 

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}