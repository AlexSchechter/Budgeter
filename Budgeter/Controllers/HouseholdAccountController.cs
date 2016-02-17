using Budgeter.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    public class HouseholdAccountController : Controller
    {
        //Get /HouseholdAccount/Index
        public ActionResult Index()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string userId = User.Identity.GetUserId();
            int householdId = db.Users.FirstOrDefault(u => u.Id == userId).HouseholdId;
            List<HouseholdAccount> model = db.HouseholdAccounts.Where(h => h.HouseholdId == householdId).ToList();
            ViewBag.HouseholdName = db.Households.FirstOrDefault(h => h.Id == householdId).Name;
            return View(model);
        }

        //GET:  /HouseholdAccount/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        //POST:  /HouseholdAccount/Create
        [HttpPost]
        public async Task<ActionResult> Create(HouseholdAccount model)
        {
            if (ModelState.IsValid)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                string userId = User.Identity.GetUserId();
                HouseholdAccount household = new HouseholdAccount
                {
                    Name = model.Name,
                    Balance = model.Balance,
                    ReconciledBalance = model.ReconciledBalance,
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
        public ActionResult Edit(int householdAccountId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            HouseholdAccount model = db.HouseholdAccounts.FirstOrDefault(h => h.Id == householdAccountId);
            return View(model);
        }
        
        //POST: /HouseholdAccount/Edit
        [HttpPost]
        public async Task<ActionResult> Edit(HouseholdAccount model)
        {
            if(ModelState.IsValid)
            { 
                ApplicationDbContext db = new ApplicationDbContext();
                HouseholdAccount householdAccount = db.HouseholdAccounts.FirstOrDefault(h => h.Id == model.Id);
                householdAccount.Name = model.Name;
                householdAccount.Balance = model.Balance;
                householdAccount.ReconciledBalance = model.ReconciledBalance;
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        //GET: /HouseholdAccount/Delete
        [HttpGet]
        public ActionResult Delete(int householdAccountId)
        {
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