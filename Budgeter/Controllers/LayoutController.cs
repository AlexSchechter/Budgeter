using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
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