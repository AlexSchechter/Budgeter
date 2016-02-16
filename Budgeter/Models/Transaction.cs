using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int HouseholdAccountId { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }
        public double Amount { get; set; }
        public int CategoryId { get; set; }
        public int ReconciledAmount { get; set; }
        public bool Reconciled { get; set; }
        [InverseProperty("Transactions")]
        public string EnteredById { get; set; }

        public virtual HouseholdAccount HouseholdAccount { get; set; }
        public virtual Category Category { get; set; }
        public virtual ApplicationUser EnteredBy { get; set; }
    }
}