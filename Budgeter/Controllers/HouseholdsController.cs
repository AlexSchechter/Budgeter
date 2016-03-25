using Budgeter.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    [Authorize]
    [RequireHttps]
    public class HouseholdsController : BaseController
    {
        //Get /Home/Households
        public async Task<ActionResult> Index ()
        {
            ApplicationUser user = GetUserInfo();
            Household household = GetHouseholdInfo();           
            IEnumerable<Invitation> invitations = db.Invitations.Where(i => i.Email == user.Email);
            List<InvitationWithHousehold> householdOptions = new List<InvitationWithHousehold>();
            foreach (Invitation invitation in invitations)
            {
                Household householdFromInvite = await db.Households.FindAsync(invitation.HouseholdId);
                if (householdFromInvite.Id != household.Id && householdFromInvite.MarkedForDeletion == false)
                {
                    householdOptions.Add(new InvitationWithHousehold
                    {
                        Invitation = invitation,
                        Household = householdFromInvite
                    });
                }               
            }

            return View(new HouseholdViewModel
            {
                CurrentHousehold = household,
                CombinedBudgetAmounts = db.BudgetItems.Where(b => b.Budget.HouseholdId == household.Id).ToList().Sum(b => b.Amount),
                TotalBalance = db.Transactions.Where(t => t.HouseholdAccount.HouseholdId == household.Id).ToList().Sum(t => t.Amount),
                HouseholdOptions = householdOptions.OrderBy(h => h.Household.Name).ToList()
            });
        }

        //GET
        [HttpGet]
        public ActionResult ChangeHousehold(int? invitationId)
        {
            Household model = GetHouseholdInfo();
            if (invitationId == null)
                return RedirectToAction("Index", "Home");

            ViewBag.invitationId = invitationId;
            return View(model);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeHousehold(string submitButton, int? invitationId)
        {
            if (User.Identity.Name != DemoEmail)
            {
                if (invitationId == null || !db.Invitations.Any(i => i.Id == invitationId))
                    return HttpNotFound();

                Invitation invitation = await db.Invitations.FindAsync(invitationId);
                ApplicationUser user = GetUserInfo();
                Household newHousehold = await db.Households.FindAsync(invitation.HouseholdId);

                if (submitButton == "Confirm" && user.Email == invitation.Email && newHousehold.MarkedForDeletion == false)
                {
                    Household oldHousehold = GetHouseholdInfo();
                    if (oldHousehold.Members.Count == 1)
                        oldHousehold.MarkedForDeletion = true;

                    user.HouseholdId = newHousehold.Id;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Households");
                }
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
                return View(GetHouseholdInfo());
            }
            return RedirectToAction("Index", "Households");
        }

        //POST: /Manage/CreateAndChangeHousehold
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAndChangeHousehold(string submitButton, string newName)
        {
            if (submitButton == "Confirm" && newName != null && newName != "" && User.Identity.Name != DemoEmail)
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

                PopulateCategories(newHousehold.Id);                    
            }
            return RedirectToAction("Index", "Households");
        }
    }
}