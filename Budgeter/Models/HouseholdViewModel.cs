using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class HouseholdViewModel
    {
        public Household CurrentHousehold { get; set; }
        public List<Household> HouseholdOptions { get; set; }
    }
}