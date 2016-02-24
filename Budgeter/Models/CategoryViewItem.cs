using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class CategoryViewItem
    {
        public Category Category { get; set; }
        public int TransactionCount { get; set; }
    }
}