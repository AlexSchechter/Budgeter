using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budgeter.Models;

namespace Budgeter.Controllers
{
    public class BudgetsController : AppController
    {
       
        // GET: Budgets
        public async Task<ActionResult> Index()
        {
            Household household = HouseholdInfo();
            ViewBag.HouseholdName = household.Name;
            ViewBag.BudgetAmount = "100";
            var budgets = db.Budgets.Where(b => b.HouseholdId == household.Id);
            return View(await budgets.ToListAsync());
        }

        // GET: Budgets/Details/5
        public async Task<ActionResult> Details(int? BudgetId)
        {
            if (BudgetId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = await db.Budgets.FindAsync(BudgetId);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // GET: Budgets/Create
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
            if (ModelState.IsValid)
            {
                budget.HouseholdId = HouseholdInfo().Id;
                db.Budgets.Add(budget);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budget.HouseholdId);
            return View(budget);
        }

        // GET: Budgets/Edit/5
        public async Task<ActionResult> Edit(int? BudgetId)
        {
            if (BudgetId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = await db.Budgets.FindAsync(BudgetId);
            if ((budget == null) || (budget.HouseholdId != HouseholdInfo().Id))
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,HouseholdId")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budget).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budget.HouseholdId);
            return View(budget);
        }

        // GET: Budgets/Delete/5
        public async Task<ActionResult> Delete(int? BudgetId)
        {
            if (BudgetId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = await db.Budgets.FindAsync(BudgetId);
            if ((budget == null) || (budget.HouseholdId != HouseholdInfo().Id))
                return HttpNotFound();

            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int BudgetId)
        {          
            Budget budget = await db.Budgets.FindAsync(BudgetId);
            if (budget.HouseholdId != HouseholdInfo().Id)
                return HttpNotFound();

            db.Budgets.Remove(budget);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
