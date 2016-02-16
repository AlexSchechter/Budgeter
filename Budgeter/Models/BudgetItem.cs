using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class BudgetItem
    {
        public BudgetItem()
        {
            this.Budgets = new HashSet<Budget>();
        }
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public double Amount { get; set; }

        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual Category Category { get; set; }
    }
}