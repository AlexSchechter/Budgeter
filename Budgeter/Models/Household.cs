﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class Household
    {
        public Household()
        {
            this.Accounts = new HashSet<Account>();
            this.Categories = new HashSet<Category>();
            this.Budgets = new HashSet<Budget>();
            this.Members = new HashSet<ApplicationUser>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<ApplicationUser> Members { get; set; }

    }
}