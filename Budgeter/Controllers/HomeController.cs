using Budgeter.Models;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Data.Entity;

namespace Budgeter.Controllers
{
    [Authorize]
    [RequireHttps]
    public class HomeController : BaseController
    {
        public ActionResult Index(string submitButton, DateTimeOffset? date )
        {
            Household household = GetHouseholdInfo();                    
            ViewBag.Household = household.Name;
            DateTimeOffset selectedDate = new DateTimeOffset();
            if (date == null)
                selectedDate = DateTime.Now;
            else
            {
                switch (submitButton)
                {
                    case "Next Month":
                        selectedDate = date.Value.AddMonths(1);
                        break;
                    case "Previous Month":
                        selectedDate = date.Value.AddMonths(-1);
                        break;
                    default:
                        View(date);
                        break;
                }                                                
            }

            ViewBag.IsCurrentMonth = (selectedDate.Month == DateTimeOffset.Now.Month && selectedDate.Year == DateTimeOffset.Now.Year) ? true : false;

            HomeViewModel model = new HomeViewModel
            {
                ChartData = db.Categories.Where(c => c.Households.Any(h => h.Id == household.Id)).ToList()
                                            .Select(c => CategoryToChartItem(c, household.Id, selectedDate)),
                LastTransactions = db.Transactions.Where(t => t.HouseholdAccount.HouseholdId == household.Id).Include(t => t.HouseholdAccount)
                                                    .OrderByDescending(t => t.Date).Take(5),
                HouseholdAccounts = db.HouseholdAccounts.Where(h => h.HouseholdId == household.Id),
                Date = selectedDate
            };
            return View(model);
        }

        private ChartItem CategoryToChartItem(Category category, int householdId, DateTimeOffset date)
        {
           
            var transactions = db.Transactions.Where(t => t.HouseholdAccount.HouseholdId == householdId && t.Category.Id == category.Id)
                                              .Where(t => t.Amount < 0)
                                              .Where(t => t.Date.Month == date.Month && t.Date.Year == date.Year);
            var budgetItems = db.BudgetItems.Where(b => b.Budget.HouseholdId == householdId && b.Category.Id == category.Id);

            if ((!transactions.Any() || transactions == null) && (!budgetItems.Any() || budgetItems == null))
                return null;

            return new ChartItem
            {
                CategoryName = category.Name,
                AmountSpent = transactions == null || !transactions.Any() ? 0 : -transactions.Sum(t => t.Amount),
                AmountBudgeted = budgetItems == null || !budgetItems.Any() ? 0 : budgetItems.Sum(b => b.Amount)
            };
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        //GET /Home/UserProfile
        public ActionResult UserProfile()
        {
            ApplicationUser user = GetUserInfo();
            return View(new ProfileViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                HouseholdName = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId).Name
            });
        }
    }
}