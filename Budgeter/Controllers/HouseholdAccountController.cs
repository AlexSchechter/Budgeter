using Budgeter.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    [Authorize]
    [RequireHttps]
    public class HouseholdAccountController : BaseController
    {
        //Get /HouseholdAccount/Index
        public async Task<ActionResult> Index()
        {
            Household household = GetHouseholdInfo();
            //List<HouseholdAccount> model = await db.Database.SqlQuery<HouseholdAccount>("EXEC GetHouseholdAccountsForHousehold @householdId", new SqlParameter("householdId", household.Id)).ToListAsync();
            List<HouseholdAccount> model = await db.HouseholdAccounts.Where(h => h.HouseholdId == household.Id).OrderBy(h => h.Name).ToListAsync();
            ViewBag.HouseholdName = household.Name;
            ViewBag.CombinedBalance = model.Sum(h => h.Balance);
            ViewBag.CombinedReconciledBalance = model.Sum(h => h.ReconciledBalance);
            return View(model);
        }

        //GET:  /HouseholdAccount/Create
        [HttpGet]
        [ChildActionOnly]
        public ActionResult Create()
        {
            return View();
        }

        //POST:  /HouseholdAccount/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HouseholdAccount model)
        {
            if (ModelState.IsValid && User.Identity.Name != DemoEmail)
            {
                string userId = GetUserInfo().Id;
                HouseholdAccount household = new HouseholdAccount
                {
                    Name = model.Name,
                    Balance = 0,
                    ReconciledBalance = 0,
                    CreationDate = DateTimeOffset.Now,
                    HouseholdId = db.Users.FirstOrDefault(u => u.Id == userId).HouseholdId,
                };
                db.HouseholdAccounts.Add(household);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        //GET: /HouseholdAccount/Edit
        [HttpGet]
        public async Task<ActionResult> Edit(int? householdAccountId)
        {
            if (householdAccountId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseholdAccount householdAccount = await db.HouseholdAccounts.FindAsync(householdAccountId);

            if (householdAccount == null || householdAccount.HouseholdId != GetHouseholdInfo().Id)
                return HttpNotFound();

            return View(householdAccount);
        }

        //POST: /HouseholdAccount/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(HouseholdAccount model)
        {
            if (ModelState.IsValid && User.Identity.Name != DemoEmail && model.HouseholdId == GetHouseholdInfo().Id)
            {
                HouseholdAccount householdAccount = await db.HouseholdAccounts.FindAsync(model.Id);
                householdAccount.Name = model.Name;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        //GET: /HouseholdAccount/Delete
        [HttpGet]
        public ActionResult Delete(int? householdAccountId)
        {
            if (householdAccountId == null)
                return RedirectToAction("Index", "Home");

            ViewBag.householdAccountId = householdAccountId;
            ViewBag.HouseholdAccountName = db.HouseholdAccounts.Find(householdAccountId).Name;
            return View();
        }

        //POST: /HouseholdAccount/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string submitButton, int householdAccountId)
        {
            if (User.Identity.Name != DemoEmail)
            {
                HouseholdAccount householdAccount = db.HouseholdAccounts.FirstOrDefault(h => h.Id == householdAccountId);
                if (householdAccount.HouseholdId == GetHouseholdInfo().Id)
                {
                    switch (submitButton)
                    {
                        case "Delete":
                            db.HouseholdAccounts.Remove(householdAccount);
                            await db.SaveChangesAsync();
                            return RedirectToAction("Index");
                        case "Cancel":
                            return RedirectToAction("Index");
                        default:
                            return View(householdAccountId);
                    }
                }
            }       
            return RedirectToAction("Index");
        }
    }
}