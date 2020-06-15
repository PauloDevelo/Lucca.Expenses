using Lucca.Expense.DTOModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Lucca.Expense.EFModels
{
    public enum ExpenseCategory
    {
        Restaurant,
        Hotel,
        Misc
    }

    public partial class Expenses
    {
        public Expenses()
        {
            this.Id = Guid.NewGuid();
        }

        public Expenses(DTOExpense dtoExpense)
            :this()
        {
            // TODO use a mapper
            this.UserId = dtoExpense.UserId;
            this.PurchasedOn = dtoExpense.PurchasedOn;
            this.Currency = dtoExpense.Currency;
            this.Comment = dtoExpense.Comment;
            this.Category = (ExpenseCategory)Enum.Parse(typeof(ExpenseCategory), dtoExpense.Category);
            this.Amount = dtoExpense.Amount;
        }

        [JsonIgnore]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime PurchasedOn { get; set; }
        public string Comment { get; set; }
        public ExpenseCategory Category { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public virtual UserInfo User { get; set; }

        public DTOExpense ToDTOExpense(LuccaContext context)
        {
            context.Entry(this).Reference(expense => expense.User).Load();

            // TODO use a mapper
            return new DTOExpense()
            {
                UserId = this.UserId,
                User = $"{this.User.FirstName.Trim()} {this.User.LastName.Trim()}",
                Amount = this.Amount,
                Category = Enum.GetName(typeof(ExpenseCategory), this.Category),
                Comment = this.Comment,
                Currency = this.Currency,
                PurchasedOn = this.PurchasedOn
            };
        }
    }
}
