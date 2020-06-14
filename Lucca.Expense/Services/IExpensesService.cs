using Lucca.Expense.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lucca.Expense.Services
{
    public interface IExpensesService
    {
        Task<DTOExpense> AddExpense(DTOExpense dtoExpense);

        Task<List<DTOExpense>> GetExpenses(ExpensesQuery query);
    }
}
