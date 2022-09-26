using Split.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Split.Services
{
	public interface IExpenseService
	{
		Task<IEnumerable<Expense>> GetExpensesAsync();
		Task<Expense> GetExpenseAsync(int expenseId);
		Task<IEnumerable<Expense>> GetExpensesAsync(int eventId);

		Task<Expense> AddExpenseAsync(Expense expense);
		Task<bool> DeleteExpenseAsync(Expense expense);

		Task<Expense> UpdateExpenseAsync(Expense newExpense);

		int NumberOfGuests(Expense expense);

	}
}