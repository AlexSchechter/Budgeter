using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class Household
    {
        public Household()
        {
            this.HouseholdAccounts = new HashSet<HouseholdAccount>();
            this.Categories = new HashSet<Category>();
            this.Budgets = new HashSet<Budget>();
            this.Members = new HashSet<ApplicationUser>();
            this.Invitations = new HashSet<Invitation>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool MarkedForDeletion { get; set; }
        
        public virtual ICollection<HouseholdAccount> HouseholdAccounts { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<ApplicationUser> Members { get; set; }
        public virtual ICollection<Invitation> Invitations { get; set; }

    }
}