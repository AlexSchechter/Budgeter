using System.Collections.Generic;

namespace Budgeter.Models
{
    public class HouseholdViewModel
    {
        public Household CurrentHousehold { get; set; }
        public List<Household> HouseholdOptions { get; set; }
        public decimal TotalBalance { get; set;  }
        public decimal CombinedBudgetAmounts { get; set; }
    }
}