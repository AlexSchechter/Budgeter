using Budgeter.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    public class HouseholdsController : BaseController
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
            return View(new HouseholdViewModel
            {
                CurrentHousehold = household,
                HouseholdOptions = householdOptions.OrderBy(h => h.Name).ToList(),
                CombinedBudgetAmounts = db.BudgetItems.Where(b => b.Budget.HouseholdId == household.Id).ToList().Sum(b => b.Amount),
                TotalBalance = db.Transactions.Where(t => t.HouseholdAccount.HouseholdId == household.Id).ToList().Sum(t => t.Amount)
            });
        }

        //GET: :Manage/ChangeHousehold
        [HttpGet]
        public ActionResult ChangeHousehold(int? householdId)
        {
            Household model = GetHouseholdInfo();
            if (householdId == null || model == null)
                return RedirectToAction("Index", "Home");

            ViewBag.householdId = householdId;
            return View(model);
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
            Household model = GetHouseholdInfo();
            if (model == null)
                return RedirectToAction("Index", "Home");

            if (newName != null && newName != "")
            {
                ViewBag.newName = newName;
                return View(model);
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
                if (oldHousehold.Members.Count == 1)
                    oldHousehold.MarkedForDeletion = true;

                db.Households.Add(new Household { Name = newName, MarkedForDeletion = false });
                await db.SaveChangesAsync();

                Household newHousehold = db.Households.OrderByDescending(h => h.Id).FirstOrDefault(h => h.Name == newName);
                user.HouseholdId = newHousehold.Id;
                await db.SaveChangesAsync();

                PopulateCategories();                    
            }
            return RedirectToAction("Index", "Households");
        }
    }
}