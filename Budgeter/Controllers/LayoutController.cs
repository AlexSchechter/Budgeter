using System;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    [Authorize]
    [RequireHttps]
    public class LayoutController : BaseController
    {
        [ChildActionOnly]
        public ActionResult Budgeter()
        {
            string Name = String.Concat(GetUserInfo().FirstName, " ", GetUserInfo().LastName);
            return new ContentResult { Content = Name };
        }
    }
}