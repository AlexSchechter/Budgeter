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
    public class BudgetItemsController : AppController
    {
        // GET: BudgetItems
        public async Task<ActionResult> Index()
        {
            if (GetUserInfo() == null)
                return RedirectToAction("Index", "Home");

            var budgetItems = db.BudgetItems.Include(b => b.Budget).Include(b => b.Category);
            return View(await budgetItems.ToListAsync());
        }

        // GET: BudgetItems/Create
        public async Task<ActionResult> Create(int? budgetId)
        {
            if (GetUserInfo() == null)
                return RedirectToAction("Index", "Home");

            if (budgetId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Budget budget = await db.Budgets.FindAsync(budgetId);
            if ((budget == null) || budget.HouseholdId != GetHouseholdInfo().Id)
                return HttpNotFound();

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View(new BudgetItem { BudgetId = (int)budgetId});
        }

        // POST: BudgetItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CategoryId,BudgetId,Amount,Description")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid)
            {
                db.BudgetItems.Add(budgetItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Budgets", new { budgetId = budgetItem.BudgetId });
            }

            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budgetItem.CategoryId);
            return View(budgetItem);
        }

        // GET: BudgetItems/Edit/5
        public async Task<ActionResult> Edit(int? budgetItemId)
        {
            if (GetUserInfo() == null)
                return RedirectToAction("Index", "Home");

            if (budgetItemId == null)           
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            BudgetItem budgetItem = await db.BudgetItems.FindAsync(budgetItemId);
            if (budgetItem == null || budgetItem.Budget.HouseholdId != GetHouseholdInfo().Id)           
                return HttpNotFound();
            
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budgetItem.CategoryId);
            return View(budgetItem);
        }

        // POST: BudgetItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CategoryId,BudgetId,Amount,Description")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budgetItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Budgets", new { budgetId = budgetItem.BudgetId });
            }
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budgetItem.CategoryId);
            return View(budgetItem);
        }

        // GET: BudgetItems/Delete/5
        public async Task<ActionResult> Delete(int? budgetItemId)
        {
            if (GetUserInfo() == null)
                return RedirectToAction("Index", "Home");

            if (budgetItemId == null)           
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            BudgetItem budgetItem = await db.BudgetItems.FindAsync(budgetItemId);
            if (budgetItem == null || budgetItem.Budget.HouseholdId != GetHouseholdInfo().Id)           
                return HttpNotFound();
            
            return View(budgetItem);
        }

        // POST: BudgetItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int budgetItemId)
        {
            BudgetItem budgetItem = await db.BudgetItems.FindAsync(budgetItemId);
            db.BudgetItems.Remove(budgetItem);
            await db.SaveChangesAsync();
            return RedirectToAction("Details", "Budgets", new { budgetId = budgetItem.BudgetId });
        }
    }
}
