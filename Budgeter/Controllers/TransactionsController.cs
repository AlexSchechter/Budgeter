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
using Microsoft.AspNet.Identity;

namespace Budgeter.Controllers
{
    public class TransactionsController : AppController
    {       
      
        // GET: Transactions
        public async Task<ActionResult> Index(int? householdAccountId)
        {
            if (householdAccountId !=null)
            {
                ViewBag.HouseholdAccountId = householdAccountId;
                ViewBag.HouseholdAccountName = db.HouseholdAccounts.Find(householdAccountId).Name;
                var transactions = db.Transactions.Where(t => t.HouseholdAccountId == householdAccountId).Include(t => t.Category).Include(t => t.EnteredBy).Include(t => t.HouseholdAccount);
                return View(await transactions.ToListAsync());
            }
            return RedirectToAction("Index", "HouseholdAccount");  
        }

        // GET: Transactions/Create
        public ActionResult Create(int householdAccountId)
        {
            int householdId = UserInfo().HouseholdId;
            ViewBag.CategoryId = new SelectList(db.Households.FirstOrDefault(h => h.Id == householdId).Categories, "Id", "Name");
            return View(new Transaction { HouseholdAccountId = householdAccountId });
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,HouseholdAccountId,Description,Date,Amount,CategoryId,ReconciledAmount,Reconciled,EnteredById")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.Date = DateTimeOffset.Now;
                transaction.EnteredById = User.Identity.GetUserId();
                db.Transactions.Add(transaction);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { householdAccountId = transaction.HouseholdAccountId });
            }
            return View(transaction.HouseholdAccountId);
        }

        // GET: Transactions/Edit/5
        public async Task<ActionResult> Edit(int? transactionId)
        {
            if (transactionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = await db.Transactions.FindAsync(transactionId);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            int householdId = UserInfo().HouseholdId;
            ViewBag.CategoryId = new SelectList(db.Households.FirstOrDefault(h => h.Id == householdId).Categories, "Id", "Name", transaction.CategoryId);
            //ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,HouseholdAccountId,Description,Date,Amount,CategoryId,ReconciledAmount,Reconciled,EnteredById")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { householdAccountId = transaction.HouseholdAccountId });
            }
            int householdId = UserInfo().HouseholdId;
            ViewBag.CategoryId = new SelectList(db.Households.FirstOrDefault(h => h.Id == householdId).Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<ActionResult> Delete(int? transactionId)
        {
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
        public async Task<ActionResult> DeleteConfirmed(int transactionId)
        {
            Transaction transaction = await db.Transactions.FindAsync(transactionId);
            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { householdAccountId = transaction.HouseholdAccountId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
