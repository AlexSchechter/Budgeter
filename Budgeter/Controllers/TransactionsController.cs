using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Budgeter.Models;
using Microsoft.AspNet.Identity;

namespace Budgeter.Controllers
{
    [RequireHttps]
    public class TransactionsController : BaseController
    {
        // GET: Transactions/Index
        public async Task<ActionResult> Index(int? householdAccountId)
        {
           
            if (householdAccountId == null)
                return RedirectToAction("Index", "HouseholdAccount");

            HouseholdAccount householdAccount = db.HouseholdAccounts.Find(householdAccountId);
            Household household = GetHouseholdInfo();

            if (household == null ||  household.Id != householdAccount.HouseholdId)
                return RedirectToAction("Index", "Home");

            ViewBag.HouseholdAccountId = householdAccountId;
            ViewBag.HouseholdAccountName = db.HouseholdAccounts.Find(householdAccountId).Name;
            var transactions = db.Transactions.Where(t => t.HouseholdAccountId == householdAccountId).Include(t => t.Category)
                                                .Include(t => t.EnteredBy).Include(t => t.HouseholdAccount).OrderByDescending(t => t.Date);
            return View(await transactions.ToListAsync());
        }

        // GET: Transactions/Create
        public ActionResult Create(int? householdAccountId)
        {
            Household household = GetHouseholdInfo();
            if (household == null || householdAccountId == null)
                return RedirectToAction("Index", "Home");
          
            ViewBag.CategoryId = new SelectList(db.Households.Find(household.Id).Categories.OrderBy(c => c.Name), "Id", "Name");
            return View(new Transaction
            {
                HouseholdAccountId = (int)householdAccountId,
                Reconciled = true
            });
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,HouseholdAccountId,Description,Date,Amount,CategoryId,ReconciledAmount,Reconciled,EnteredById")] Transaction transaction)
        {
            if (!ModelState.IsValid)
                return View(transaction);

            HouseholdAccount householdAccount = await db.HouseholdAccounts.FindAsync(transaction.HouseholdAccountId);
            if (householdAccount.HouseholdId != GetHouseholdInfo().Id)
                return RedirectToAction("Index", "Home");
                
            transaction.Date = DateTimeOffset.Now;
            transaction.EnteredById = User.Identity.GetUserId();
            db.Transactions.Add(transaction);
            householdAccount.Balance += transaction.Amount;
            householdAccount.ReconciledBalance += transaction.ReconciledAmount;
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { householdAccountId = transaction.HouseholdAccountId });            
        }

        // GET: Transactions/Edit/5
        public async Task<ActionResult> Edit(int? transactionId)
        {
            if (transactionId == null)            
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Transaction transaction = await db.Transactions.FindAsync(transactionId);
            int householdId = GetHouseholdInfo().Id;

            if (transaction == null || transaction.HouseholdAccount.HouseholdId != householdId)          
                return HttpNotFound();
                     
            ViewBag.CategoryId = new SelectList(db.Households.Find(householdId).Categories.OrderBy(c => c.Name), "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,HouseholdAccountId,Description,Date,Amount,CategoryId,ReconciledAmount,Reconciled,EnteredById")] Transaction transaction)
        {
            Household household = GetHouseholdInfo();
            
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(db.Households.FirstOrDefault(h => h.Id == household.Id).Categories, "Id", "Name", transaction.CategoryId);
                return View(transaction);
            }

            HouseholdAccount householdAccount = await db.HouseholdAccounts.FindAsync(transaction.HouseholdAccountId);

            if (household.Id != householdAccount.HouseholdId)
                return RedirectToAction("Index", "Home");

            db.Entry(transaction).State = EntityState.Modified;
            await db.SaveChangesAsync();               
            householdAccount.Balance = 0; //Update balance
            householdAccount.ReconciledBalance = 0;
            var transactions = db.Transactions.Where(t => t.HouseholdAccountId == transaction.HouseholdAccountId);
            foreach (Transaction t in transactions)
            {
                householdAccount.Balance += t.Amount;
                householdAccount.ReconciledBalance += t.ReconciledAmount;
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { householdAccountId = transaction.HouseholdAccountId });                       
        }

        // GET: Transactions/Delete/5
        public async Task<ActionResult> Delete(int? transactionId)
        {
            if (GetUserInfo() == null)
                return RedirectToAction("Index", "Home");

            if (transactionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = await db.Transactions.FindAsync(transactionId);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? transactionId)
        {
            if (transactionId == null)
                return View(transactionId);
         
            Transaction transaction = await db.Transactions.FindAsync(transactionId);           
            HouseholdAccount account = await db.HouseholdAccounts.FirstOrDefaultAsync(h => h.Id == transaction.HouseholdAccountId);

            if (account.HouseholdId != GetHouseholdInfo().Id)
                return RedirectToAction("Index", "Home");

            account.Balance -= transaction.Amount;
            account.ReconciledBalance -= transaction.ReconciledAmount;
            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { householdAccountId = transaction.HouseholdAccountId });
        }
    }
}
