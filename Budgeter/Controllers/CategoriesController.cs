using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Budgeter.Models;
using System.Collections.Generic;

namespace Budgeter.Controllers
{
    public class CategoriesController : BaseController
    {
       
        // GET: Categories
        public ActionResult Index()
        {
            Household household = GetHouseholdInfo();
            CategoryViewModel model = new CategoryViewModel();
            CategoryViewItem item = new CategoryViewItem();
            foreach (Category category in db.Households.Find(GetUserInfo().HouseholdId).Categories)
            {
                item.Category = category;
                item.TransactionCount = category.Transactions.Where(t => t.HouseholdAccount.HouseholdId == household.Id).Count();
                model.CategoryItems.Add(item//new CategoryViewItem
                //{
                //    Category = category,
                //    TransactionCount = category.Transactions.Where(t => t.HouseholdAccount.HouseholdId == household.Id).Count()
                //}
                );
            }

            return View(model); //what is passed here the object or just the reference to the database???
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                Household household = GetHouseholdInfo();

                if (household.Categories.Any(c => c.Name == category.Name))
                    return RedirectToAction("Index");

                Category sameNameCategory = db.Categories.FirstOrDefault(c => c.Name == category.Name);
                if (sameNameCategory == null)
                {
                    db.Categories.Add(category);
                    household.Categories.Add(category);
                }
                else
                    household.Categories.Add(sameNameCategory);

                await db.SaveChangesAsync();
                return RedirectToAction("Index");           
            }

            return View();
        }

        // GET: Categories/Delete/5
        public async Task<ActionResult> Delete(int? categoryId)
        {
            if (categoryId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
         
            if (!GetHouseholdInfo().Categories.Any(c => c.Id == categoryId))
                return HttpNotFound();

            return View(await db.Categories.FindAsync(categoryId));
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int categoryId)
        {
            Household household = GetHouseholdInfo();

            if (!household.Categories.Any(c => c.Id == categoryId))
                 return HttpNotFound();

            Category category = await db.Categories.FindAsync(categoryId);
            
            if (household.HouseholdAccounts.SelectMany(t => t.Transactions).Where(t => t.CategoryId == categoryId).Count() == 0)
            {
                household.Categories.Remove(category);
                await db.SaveChangesAsync();
                if (!db.Households.SelectMany(c => c.Categories).Select(c => c.Id).Contains(categoryId))
                {
                    db.Categories.Remove(category);
                    await db.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

    }
}
