namespace Budgeter.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int HouseholdId { get; set; }
    }
}