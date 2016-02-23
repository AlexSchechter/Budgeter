using System.Collections.Generic;

namespace Budgeter.Models
{
    public class HomeViewModel
    {
        public IEnumerable<ChartItem> ChartData { get; set; }
        public IEnumerable<Transaction> LastTransactions { get; set; }
    }
}