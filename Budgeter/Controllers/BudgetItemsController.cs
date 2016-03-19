using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Budgeter.Models;

namespace Budgeter.Controllers
{
    [RequireHttps]
    public class BudgetItemsController : BaseController
    {
        // GET: BudgetItems
        public async Task<ActionResult> Index()
        {
            if (GetUserInfo() == null)
                return RedirectToAction("Index", "Home");         
            return View(await db.BudgetItems.Include(b => b.Budget).Include(b => b.Category).OrderBy(b => b.Description).ToListAsync());
        }

        // GET: BudgetItems/Create
        public async Task<ActionResult> Create(int? budgetId)
        {
            if (GetUserInfo() == null)
                return RedirectToAction("Index", "Home");

            if (budgetId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Budget budget = await db.Budgets.FindAsync(budgetId);
            Household household = GetHouseholdInfo();
            if ((budget == null) || budget.HouseholdId != household.Id)
                return HttpNotFound();

            ViewBag.CategoryId = new SelectList(household.Categories.OrderBy(c => c.Name), "Id", "Name");
            return View(new BudgetItem { BudgetId = (int)budgetId});
        }

        // POST: BudgetItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CategoryId,BudgetId,Amount,Description")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid && db.Budgets.Find(budgetItem.BudgetId).HouseholdId == GetHouseholdInfo().Id)
            {
                db.BudgetItems.Add(budgetItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Budgets", new { budgetId = budgetItem.BudgetId });
            }         
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
            Household household = GetHouseholdInfo();
            if (budgetItem == null || budgetItem.Budget.HouseholdId != GetHouseholdInfo().Id)           
                return HttpNotFound();
            
            ViewBag.BudgetId = new SelectList(household.Budgets.OrderBy(b => b.Name), "Id", "Name", budgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(household.Categories.OrderBy(c => c.Name), "Id", "Name", budgetItem.CategoryId);
            return View(budgetItem);
        }

        // POST: BudgetItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CategoryId,BudgetId,Amount,Description")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid && db.Budgets.Find(budgetItem.BudgetId).HouseholdId == GetHouseholdInfo().Id)
            {
                db.Entry(budgetItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Budgets", new { budgetId = budgetItem.BudgetId });
            }
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

            ViewBag.BudgetItemId = budgetItem.Id;
            ViewBag.BudgetId = budgetItem.Budget.Id;           
            return View();
        }

        // POST: BudgetItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int budgetItemId)
        {
            BudgetItem budgetItem = await db.BudgetItems.FindAsync(budgetItemId);
            if (db.Budgets.Find(budgetItem.BudgetId).HouseholdId == GetHouseholdInfo().Id)
            {
                db.BudgetItems.Remove(budgetItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Budgets", new { budgetId = budgetItem.BudgetId });
            }
            return View(budgetItemId);
        }
    }
}
