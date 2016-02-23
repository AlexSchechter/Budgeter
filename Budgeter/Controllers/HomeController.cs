using Budgeter.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;

namespace Budgeter.Controllers
{
    [RequireHttps]
    public class HomeController : AppController
    {
        public ActionResult Index()
        {         
            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser user = GetUserInfo();
                Household household = GetHouseholdInfo();
                ViewBag.Name = string.Concat(user.FirstName, " ", user.LastName);
                HomeViewModel model = new HomeViewModel
                {
                    ChartData = db.Categories.Where(c => c.Households.Any(h => h.Id == household.Id )).ToList().Select(c => CategoryToChartItem(c, household.Id)).ToList()
                };
                return View(model);
            }               
            return RedirectToAction("Login", "Account");
        }

        private ChartItem CategoryToChartItem(Category category, int householdId)
        {
           
            var transactions = db.Transactions.Where(t => t.HouseholdAccount.HouseholdId == householdId && t.Category.Id == category.Id)
                                              .Where(t => t.Amount < 0)
                                              .Where(t => t.Date.Month == DateTimeOffset.Now.Month && t.Date.Year == DateTimeOffset.Now.Year);
            var budgetItems = db.BudgetItems.Where(b => b.Budget.HouseholdId == householdId && b.Category.Id == category.Id);

            return new ChartItem
            {
                CategoryName = category.Name,
                AmountSpent = transactions == null || !transactions.Any() ? 0 : -transactions.Sum(t => t.Amount),
                AmountBudgeted = budgetItems == null || !budgetItems.Any() ? 0 : budgetItems.Sum(b => b.Amount)
            };
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

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