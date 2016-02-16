using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class ChangeHousehold
    {
        public Household OldHousehold { get; set; }
        public Household NewHousehold { get; set; }
    }
}