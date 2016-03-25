using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Budgeter.Models;
using System.Collections.Generic;

namespace Budgeter.Controllers
{
    [Authorize]
    [RequireHttps]
    public class ManageController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //GET: /Manage/EditProfile
        [HttpGet]
        [ChildActionOnly]
        public ActionResult EditProfile()
        {        
            ApplicationUser user = GetUserInfo();
            if (user == null)
                return RedirectToAction("Index", "Home");

            Profile model = new Profile
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
            };

            return View(model);
        }

        //POST: /Manage/EditProfile
        [HttpPost]
        public async Task<ActionResult> EditProfile(Profile profile)
        {
            ApplicationUser user = GetUserInfo();
            if (profile == null || user.Email == DemoEmail)
                RedirectToAction("Index", "Home");

            List<Invitation> invitations = db.Invitations.Where(i => i.Email == user.Email).ToList();
            user.UserName = profile.Username;
            user.Email = profile.Email;
            user.FirstName = profile.FirstName;
            user.LastName = profile.LastName;
            foreach (Invitation invitation in invitations)
                invitation.Email = profile.Email;
            await db.SaveChangesAsync();
            return RedirectToAction("UserProfile", "Home");
        }
    }
}