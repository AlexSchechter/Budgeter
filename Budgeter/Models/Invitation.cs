using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int HouseholdId { get; set; }
        public string UserId { get; set; }
    }
}