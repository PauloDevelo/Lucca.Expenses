using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lucca.Expense.EFModels;
using System.Linq.Expressions;
using Lucca.Expense.DTOModels;
using Lucca.Expense.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Lucca.Expense.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpensesService expensesService;

        public ExpensesController(IExpensesService expensesService)
        {
            this.expensesService = expensesService;
        }

        // GET: api/Expenses/5
        [HttpGet]
        public async Task<ActionResult<List<DTOExpense>>> GetExpenses(Guid? userId, ExpenseAscSort? sort)
        {
            var query = new ExpensesQuery() { UserId = userId, Sort = sort };
            var expenses = await this.expensesService.GetExpenses(query);
            return expenses;
        }

        // PUT: api/Expenses/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpenses(Guid id, Expenses expense)
        {
            return new StatusCodeResult(StatusCodes.Status501NotImplemented);
        }


        // POST: api/Expenses
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<DTOExpense>> PostExpenses(DTOExpense expense)
        {
            var expenseAdded = await this.expensesService.AddExpense(expense);
            return expenseAdded;
        }

        // DELETE: api/Expenses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Expenses>> DeleteExpenses(Guid id)
        {
            return new StatusCodeResult(StatusCodes.Status501NotImplemented);
        }
    }
}
