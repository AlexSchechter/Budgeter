using System;
using System.Collections.Generic;

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
        public decimal Balance { get; set; }
        public decimal ReconciledBalance { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreationDate { get; set; }   
            
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual Household Household { get; set; }
    }
}