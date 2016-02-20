using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class HouseholdAccount
    {
        public HouseholdAccount()
        {
            this.Transactions = new HashSet<Transaction>();
        }
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public double Balance { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public double ReconciledBalance { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual Household Household { get; set; }

    }
}