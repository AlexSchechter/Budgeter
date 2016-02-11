using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }
        public double Amount { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int ReconciledAmount { get; set; }
        public bool Reconciled { get; set; }
    }
}