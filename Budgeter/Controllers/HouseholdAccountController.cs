using Budgeter.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    public class HouseholdAccountController : AppController
    {
        //Get /HouseholdAccount/Index
        public ActionResult Index()
        {
            Household household = GetHouseholdInfo();
            if (household == null)
                return RedirectToAction("Index", "Home");

            List<HouseholdAccount> model = db.HouseholdAccounts.Where(h => h.HouseholdId == household.Id).ToList();
            ViewBag.HouseholdName = household.Name;
            return View(model);
        }

        //GET:  /HouseholdAccount/Create
        [HttpGet]
        public ActionResult Create()
        {
            if (GetUserInfo() == null)
                RedirectToAction("Index", "Home");

            return View();
        }

        //POST:  /HouseholdAccount/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HouseholdAccount model)
        {
            if (ModelState.IsValid)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                string userId = User.Identity.GetUserId();
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
            return View();
        }

        //GET: /HouseholdAccount/Edit
        [HttpGet]
        public async Task<ActionResult> Edit(int? householdAccountId)
        {
            if (GetUserInfo() == null)
                return RedirectToAction("Index", "Home");

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
            if(ModelState.IsValid)
            { 
                ApplicationDbContext db = new ApplicationDbContext();
                HouseholdAccount householdAccount = await db.HouseholdAccounts.FindAsync(model.Id);
                householdAccount.Name = model.Name;
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        //GET: /HouseholdAccount/Delete
        [HttpGet]
        public ActionResult Delete(int householdAccountId)
        {
            if (GetUserInfo() == null)
                return RedirectToAction("Index", "Home");

            ViewBag.householdAccountId = householdAccountId;
            return View();
        }

        //DELETE: /HouseholdAccount/Delete
        [HttpPost]
        public ActionResult Delete(string submitButton, int householdAccountId)
        {
            switch (submitButton)
            {
                case "Delete":
                    ApplicationDbContext db = new ApplicationDbContext();
                    HouseholdAccount householdAccount = db.HouseholdAccounts.FirstOrDefault(h => h.Id == householdAccountId);
                    db.HouseholdAccounts.Remove(householdAccount);
                    db.SaveChangesAsync();
                    return RedirectToAction("Index");
                case "Cancel":
                    return RedirectToAction("Index");
                default:
                    return View(householdAccountId);
            }

        }
    }
}