using Microsoft.EntityFrameworkCore;
using Split.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split.Data
{
    public class ExpenseRepository : Repository<Expense>, IExpenseRepository
    {
		public ExpenseRepository(AppDbContext context) : base(context)
		{

		}

		public async Task<IEnumerable<Expense>> GetExpensesFromEventAsync(int EventId)
		{
			var result = await Context.Expenses
								.Where(ex => ex.EventRef == EventId)
								.Include(ex => ex.CostAllocations)
								.ToListAsync();

			return result;
		}
	}
}
