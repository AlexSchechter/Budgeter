using Budgeter.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    public class HouseholdsController : AppController
    {
        //Get /Home/Households
        public ActionResult Index ()
        {
            ApplicationUser user = GetUserInfo();
            Household household = GetHouseholdInfo();
            if (household == null)
                return RedirectToAction("Index", "Home");
            List<int> householdOptionsIds = db.Invitations.Where(i => i.Email == user.Email).
                Where(i => i.HouseholdId != household.Id).Select(i => i.HouseholdId).ToList();
            List<Household> householdOptions = new List<Household>();
            Household householdOption = new Household();
            foreach (int optionId in householdOptionsIds)
            {
                householdOption = db.Households.Find(optionId);
                if (householdOption.MarkedForDeletion == false)
                    householdOptions.Add(householdOption);
            }               
            return View(new HouseholdViewModel { CurrentHousehold = household, HouseholdOptions = householdOptions.OrderBy(h => h.Name).ToList()});
        }

        //GET: :Manage/ChangeHousehold
        [HttpGet]
        public ActionResult ChangeHousehold(int? householdId)
        {
            ViewBag.householdId = householdId;
            return View(householdId);
        }

        //POST: /Manage/ChangeHousehold
        [HttpPost]
        public async Task<ActionResult> ChangeHousehold(string submitButton, int? householdId)
        {
            if (submitButton == "Confirm")
            {
                if ((householdId != null) && (db.Households.Any(h => h.Id == householdId) && (db.Households.Find(householdId).MarkedForDeletion == false)))
                {
                    ApplicationUser user = GetUserInfo();
                    Household oldHousehold = GetHouseholdInfo();
                    user.HouseholdId = (int)householdId;
                    if (oldHousehold.Members.Count == 1)
                        oldHousehold.MarkedForDeletion = true;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Households");
                }
                return HttpNotFound();
            }
            return RedirectToAction("Index", "Households");
        }

        //GET: /Manage/CreateAndChangeHousehold
        [HttpGet]
        public ActionResult CreateAndChangeHousehold(string newName)
        {
            if (newName != null && newName != "")
            {
                ViewBag.newName = newName;
                return View();
            }
            return RedirectToAction("Index", "Households");
        }

        //POST: /Manage/CreateAndChangeHousehold
        [HttpPost]
        public async Task<ActionResult> CreateAndChangeHousehold(string submitButton, string newName)
        {
            if (submitButton == "Confirm" && newName != null && newName != "")
            {              
                ApplicationUser user = GetUserInfo();
                Household oldHousehold = GetHouseholdInfo();
                db.Households.Add(new Household { Name = newName, MarkedForDeletion = false });
                await db.SaveChangesAsync();
                Household newHousehold = db.Households.OrderByDescending(h => h.Id).FirstOrDefault(h => h.Name == newName);
                user.HouseholdId = newHousehold.Id;

                string[] categoryNames = {"Automobile", "Bank Charges", "Charity", "Childcare", "Clothing", "Credit Card Fees", "Education",
                "Events", "Food", "Gifts", "Healthcare", "Household", "Savings Interest", "Insurance", "Job Expenses", "Leisure (not holiday)", "Hobbies",
                "Loans", "Misc", "Pet Care", "Salary", "Savings", "Taxes", "Utilities", "Holiday"};

                Category myCategory = new Category();
                foreach (string name in categoryNames)
                {
                    if (!db.Categories.Any(c => c.Name == name))
                    {
                        db.Categories.Add(new Category { Name = name });
                        await db.SaveChangesAsync();
                    }

                    myCategory = db.Categories.FirstOrDefault(c => c.Name == name);
                    newHousehold.Categories.Add(myCategory);
                }
                
                if (oldHousehold.Members.Count == 1)
                    oldHousehold.MarkedForDeletion = true;

                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Households");
        }
    }
}