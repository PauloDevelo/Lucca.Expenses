using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lucca.Expense.Services
{
    public enum ExpenseErrorCode
    {
        PurchaseInFuture = 0,
        PurchaseMoreThan3MonthOld,
        CommentIsMandatory,
        DuplicatedExpense,
        ExpenseCurrencyDifferentThanUserCurrency,
        UserNotFound
    }
    public class ExpenseValidationException: Exception
    {
        private readonly Dictionary<ExpenseErrorCode, string> errors;
        public readonly string title = "Expense validation error";
        public readonly string type = "Validation error";
        
        public ExpenseValidationException(Dictionary<ExpenseErrorCode, string> errors)
        {
            this.errors = errors;
        }

        public List<ExpenseErrorCode> GetErrorCodes()
        {
            return errors.Keys.ToList();
        }

        public string GetErrorMessage(ExpenseErrorCode key)
        {
            return this.errors[key]; 
        }
    }
}
