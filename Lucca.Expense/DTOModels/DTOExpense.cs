using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lucca.Expense.DTOModels
{
    public class DTOExpense
    {
        public DTOExpense()
        {
        }

        public Guid UserId { get; set; }
        public string User { get; set; }
        public DateTime PurchasedOn { get; set; }
        public string Comment { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
