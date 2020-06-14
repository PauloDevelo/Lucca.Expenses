using Lucca.Expense.DTOModels;
using Lucca.Expense.EFModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lucca.Expense.Services
{
    public class ExpensesService : IExpensesService
    {
        private IServiceProvider serviceProvider;
        private readonly ITimeService timeService;

        public ExpensesService(IServiceProvider serviceProvider, ITimeService timeService)
        {
            this.serviceProvider = serviceProvider;
            this.timeService = timeService;
        }

        public async Task<List<DTOExpense>> GetExpenses(ExpensesQuery query) {
            using (LuccaContext context = this.serviceProvider.GetService<LuccaContext>())
            {
                var expenses = context.Expenses.AsQueryable();

                if (query.UserId.HasValue)
                {
                    expenses = context.Expenses.Where(expense => expense.UserId == query.UserId);
                }

                if (query.Sort.HasValue)
                {
                    expenses = this.AscSort(expenses, query.Sort.Value);
                }

                return await expenses.Select(expense => expense.ToDTOExpense(context)).ToListAsync();
            }
        }

        public async Task<DTOExpense> AddExpense(DTOExpense dtoExpense)
        {
            using (LuccaContext context = this.serviceProvider.GetService<LuccaContext>())
            {
                this.ThrowIfExpenseInvalid(context, dtoExpense);

                var expenseToAdd = new Expenses(dtoExpense);
                context.Expenses.Add(expenseToAdd);

                await context.SaveChangesAsync();

                return expenseToAdd.ToDTOExpense(context);
            }
        }

        private void ThrowIfExpenseInvalid(LuccaContext context, DTOExpense expense)
        {
            var errors = new Dictionary<ExpenseErrorCode, string>();

            if (expense.PurchasedOn > this.timeService.GetCurrentUtcTime())
            {
                errors.Add(ExpenseErrorCode.PurchaseInFuture, $"Expense cannot be recorded for this date {expense.PurchasedOn}");
            }
            else if (DateTime.UtcNow - expense.PurchasedOn > TimeSpan.FromDays(90))
            {
                errors.Add(ExpenseErrorCode.PurchaseMoreThan3MonthOld, $"The expense is older than 90 days: {expense.PurchasedOn}");
            }

            if (string.IsNullOrEmpty(expense.Comment))
            {
                errors.Add(ExpenseErrorCode.CommentIsMandatory, $"The expense comment is mandatory");
            }

            var user = context.UserInfo.Where(user => user.Id == expense.UserId).FirstOrDefault();
            if (user == null)
            {
                errors.Add(ExpenseErrorCode.UserNotFound, $"The user {expense.UserId} cannot be found");
            }
            else
            {
                var userExpenses = context.Expenses.Where(exp => exp.UserId == expense.UserId);
                if (userExpenses.Any(exp => exp.PurchasedOn == expense.PurchasedOn && exp.Amount == expense.Amount))
                {
                    errors.Add(ExpenseErrorCode.DuplicatedExpense, $"The expense purchased on {expense.PurchasedOn} with an amount of {expense.Amount} seems already recorded.");
                }

                if (expense.Currency != user.Currency)
                {
                    errors.Add(ExpenseErrorCode.ExpenseCurrencyDifferentThanUserCurrency, $"The expense currency {expense.Currency} should be the same than the user currency {user.Currency}");
                }
            }

            if (errors.Any())
            {
                throw new ExpenseValidationException(errors);
            }
        }

        private IQueryable<Expenses> AscSort(IQueryable<Expenses> expenses, ExpenseAscSort sort)
        {
            switch (sort)
            {
                case ExpenseAscSort.Amount:
                    return expenses.OrderBy(expense => expense.Amount);
                case ExpenseAscSort.Date:
                    return expenses.OrderBy(expense => expense.PurchasedOn);
                default:
                    throw new InvalidOperationException($"The sort with the value {sort} is not implemented");
            }
        }
    }
}
