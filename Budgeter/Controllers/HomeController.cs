﻿using Budgeter.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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
        public ActionResult UserProfile(string userId)
        {
            if (userId == null)
                return RedirectToAction("Index");

            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser user = db.Users.FirstOrDefault(u => u.Id == userId);
            ProfileViewModel model = new ProfileViewModel { Email = user.Email, FirstName = user.FirstName, LastName = user.LastName,
                Username = user.UserName, HouseholdName = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId).Name };

            return View(model);
        }

        //Get /Home/HouseholdAccounts
        public ActionResult HouseholdAccounts()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string userId = User.Identity.GetUserId();
            int householdId = db.Users.FirstOrDefault(u => u.Id == userId ).HouseholdId;
            List<HouseholdAccount> model = db.HouseholdAccounts.Where(h => h.HouseholdId == householdId).ToList();
            return View(model);
        }
    }
}