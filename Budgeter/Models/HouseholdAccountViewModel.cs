using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class HouseholdAccountViewModel
    {
        public string Name { get; set; }
        public double Balance { get; set; }
        public double ReconReconciledBalance { get; set; }
    }
}