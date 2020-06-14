using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lucca.Expense.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ExpensesExceptionHandler : ControllerBase
    {
        private readonly ILogger logger;
        public ExpensesExceptionHandler(ILogger<ExpensesExceptionHandler> logger)
        {
            this.logger = logger;
        }

        [Route("/api/expenses/error")]
        public IActionResult ExceptionHandler() {
            return Problem();
        }
    }
}
