using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lucca.Expense.Services
{
    public enum ExpenseAscSort
    {
        Amount,
        Date,
    }

    public class ExpensesQuery
    {
        public Guid? UserId { get; set; }
        public ExpenseAscSort? Sort { get; set; }
    }
}
