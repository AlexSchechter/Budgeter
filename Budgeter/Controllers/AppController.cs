using Budgeter.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    public class AppController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

        protected ApplicationUser UserInfo()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.FirstOrDefault(u => u.Id == userId);
            return user;
        }
    }
}