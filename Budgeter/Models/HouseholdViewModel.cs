using System.Collections.Generic;

namespace Budgeter.Models
{
    public class HouseholdViewModel
    {
        public Household CurrentHousehold { get; set; }
        public List<Household> HouseholdOptions { get; set; }
    }
}