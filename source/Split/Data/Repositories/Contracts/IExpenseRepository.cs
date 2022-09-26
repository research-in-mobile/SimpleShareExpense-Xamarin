using Split.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Split.Data
{
    public interface IExpenseRepository : IRepository<Expense>
    {
		Task<IEnumerable<Expense>> GetExpensesFromEventAsync(int EventId);
	}
}
