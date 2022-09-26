using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Split.Data;
using Split.Entities;

namespace Split.Services
{
	public class ExpenseService : IExpenseService
	{
		private readonly IUnitOfWork _uow;
		private readonly ILogService _logService;


		public ExpenseService(IUnitOfWork uow,
							ILogService logService)
		{
			_uow = uow;
			_logService = logService;
		}

		private int SaveChanges() => _uow.Complete();

		public async Task<IEnumerable<Expense>> GetExpensesAsync() => await _uow.Expenses.GetAllAsync();

		public async Task<Expense> GetExpenseAsync(int expenseId) => await _uow.Expenses.GetAsync(expenseId);

		public async Task<IEnumerable<Expense>> GetExpensesAsync(int eventId) => await _uow.Expenses.GetExpensesFromEventAsync(eventId);


		public async Task<Expense> AddExpenseAsync(Expense expense)
		{
			await _uow.Expenses.AddAsync(expense);

			var savedChanges = SaveChanges();

			return savedChanges > 0 ? expense : null;
		}

		public async Task<bool> DeleteExpenseAsync(Expense expense)
		{
			await _uow.Expenses.RemoveAsync(expense.ExpenseId);

			var savedChanges = SaveChanges();

			return savedChanges > 0;
		}

		public async Task<Expense> UpdateExpenseAsync(Expense newExpense)
		{
			var expense = await _uow.Expenses.GetAsync(newExpense.ExpenseId);

			expense = newExpense;

			var savedChanges = SaveChanges();

			return savedChanges > 0 ? expense : null;
		}

		public int NumberOfGuests(Expense expense) => expense?.CostAllocations != null ? expense.CostAllocations.Count : 0;

		public double ExpensesTotalCost(IList<Expense> expenses)
		{
			double total = 0;

			if (expenses == null)
				return total;

			foreach (var expense in expenses)
			{
				total += expense.Cost;
			}

			return total;
		}


	}
}
