using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lucca.Expense.Services
{
    public interface ITimeService
    {
        //
        // Summary:
        //     Gets the current system UTC time.
        //
        // Returns:
        //     A System.DateTime of the current system UTC time.
        DateTime GetCurrentUtcTime();
    }
}
