﻿using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budgeter.Models;

namespace Budgeter.Controllers
{
    [Authorize]
    [RequireHttps]
    public class BudgetsController : BaseController
    {
        // GET: Budgets
        public async Task<ActionResult> Index()
        {
            Household household = GetHouseholdInfo();      
            ViewBag.HouseholdName = household.Name;
            return View(await db.Budgets.Where(b => b.HouseholdId == household.Id).OrderBy(b => b.Name).ToListAsync());
        }

        // GET: Budgets/Details/5
        public async Task<ActionResult> Details(int? budgetId)
        {
            if (budgetId == null)            
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Budget budget = await db.Budgets.FindAsync(budgetId);
            if (budget == null || budget.HouseholdId != GetHouseholdInfo().Id)           
                return HttpNotFound();

            ViewBag.BudgetName = budget.Name;
            ViewBag.BudgetId = budgetId;
            return View(await db.BudgetItems.Where(b => b.BudgetId == budgetId).Include(b => b.Category).OrderBy(b => b.Description).ToListAsync());
        }

        // GET: Budgets/Create
        [ChildActionOnly]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,HouseholdId")] Budget budget)
        {
            if (ModelState.IsValid && User.Identity.Name != DemoEmail)
            {
                budget.HouseholdId = GetHouseholdInfo().Id;
                db.Budgets.Add(budget);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: Budgets/Edit/5
        public async Task<ActionResult> Edit(int? budgetId)
        {
            if (budgetId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Budget budget = await db.Budgets.FindAsync(budgetId);
            if ((budget == null) || (budget.HouseholdId != GetHouseholdInfo().Id))
                return HttpNotFound();

            return View(budget);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,HouseholdId")] Budget budget)
        {
            if (ModelState.IsValid && User.Identity.Name != DemoEmail)
            {
                if (budget.HouseholdId != GetHouseholdInfo().Id)
                    return HttpNotFound();

                db.Entry(budget).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }            
            return View(budget);
        }

        // GET: Budgets/Delete/5
        public async Task<ActionResult> Delete(int? budgetId)
        {
            if (budgetId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Budget budget = await db.Budgets.FindAsync(budgetId);
            if ((budget == null) || (budget.HouseholdId != GetHouseholdInfo().Id))
                return HttpNotFound();

            ViewBag.Name = budget.Name;
            ViewBag.BudgetId = budget.Id;
            return View();
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int budgetId)
        {
            if (User.Identity.Name != DemoEmail)
            {
                Budget budget = await db.Budgets.FindAsync(budgetId);
                if (budget.HouseholdId != GetHouseholdInfo().Id)
                    return HttpNotFound();

                db.Budgets.Remove(budget);
                await db.SaveChangesAsync();
            }          
            return RedirectToAction("Index");
        }
    }
}
