using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class HouseholdAccountViewModel
    {
        public HouseholdAccount HouseholdAccount { get; set; }
        public int Balance { get; set; }
        public int ReconciledBalance { get; set; }
    }
}