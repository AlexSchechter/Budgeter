using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class InvitationWithHousehold
    {
        public Invitation Invitation { get; set; }
        public Household Household { get; set; }
    }
}