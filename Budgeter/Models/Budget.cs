using System.Collections.Generic;

namespace Budgeter.Models
{
    public class Budget
    {
        public Budget()
        {
            this.BudgetItems = new HashSet<BudgetItem>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int HouseholdId { get; set; }

        public virtual ICollection<BudgetItem> BudgetItems { get; set; }
        public virtual Household Household { get; set; }
    }
}