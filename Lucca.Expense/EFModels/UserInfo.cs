using System;
using System.Collections.Generic;

namespace Lucca.Expense.EFModels
{
    public partial class UserInfo
    {
        public UserInfo()
        {
            Expenses = new HashSet<Expenses>();
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Currency { get; set; }

        public virtual ICollection<Expenses> Expenses { get; set; }
    }
}
