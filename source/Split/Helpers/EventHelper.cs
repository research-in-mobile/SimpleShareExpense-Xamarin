using Split.Entities;
using Split.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Split.Helpers
{
	public static class EventHelper
	{
		public static event EventHandler DataContextChanged;
		public static void OnDataContextChanged(object sender, EventArgs e) => DataContextChanged?.Invoke(sender, e);

		public static Dictionary<string, TransactionStrategy> TransactionStrategies = new Dictionary<string, TransactionStrategy>
		{
			{ "Host Managed Transactions", TransactionStrategy.HMT },
			{ "By Expense Transactions", TransactionStrategy.BET },
			{ "Minimum Transactions", TransactionStrategy.MT }
		};

		public static IEnumerable<Transaction> CalculateTransactions(ICollection<Expense> expenses)
		{
			var transactions = new List<Transaction>();

			foreach (var expense in expenses)
			{
				foreach (var costAllocation in expense.CostAllocations)
				{
					var transaction = new Transaction();

					transaction.Amount = costAllocation.AllocatedCost;
					transaction.Expense = expense;
					transaction.ExpenseRef = expense.ExpenseId;

					transaction.PayableUser = expense.AssignedUser;
					transaction.PayableUserRef = expense.UserRef;

					if (costAllocation.UserRef != expense.UserRef)
					{
						transaction.ReceivableUser = costAllocation.User;
						transaction.ReceivableUserRef = costAllocation.UserRef;
					}

					transactions.Add(transaction);
				}
			}

			return transactions;
		}

		public static IEnumerable<UserExpenseAggregate> CalculateByExpenseTransaction(Event evnt)
		{
			var expenses = evnt.Expenses;

			if (expenses == null)
			{
				if (evnt?.Guests == null) return null;
				return ExpenseHelper.GetUserExpenseAggregate(evnt.Guests);
			}

			var result = new List<UserExpenseAggregate>();
			bool initial = true;
			int index = 0;

			foreach (var expense in expenses)
			{
				var assignedUser = expense.CostAllocations.FirstOrDefault(ca =>
						ca.User.UserId == expense.AssignedUser.UserId);

				if (assignedUser == null)
					return null;

				IList<Transaction> transactions = ExpenseHelper.CalculateTransactions(expense).ToList();
				var expenseAllocatedUser = expense.CostAllocations.FirstOrDefault(c => c.UserRef == expense.UserRef);
				var expenseAllocatedUserIndex = expense.CostAllocations.IndexOf(expenseAllocatedUser);

				foreach (var costAllocation in expense.CostAllocations)
				{
					if (initial)
					{
						var uee = new UserExpenseAggregate();
						uee.User = costAllocation.User;
						uee.Cost = costAllocation.AllocatedCost;

						result.Add(uee);
					}
					else
					{
						result[index].Cost += costAllocation.AllocatedCost;
					}

					result[index].Spend += result[index].User.UserId == assignedUser.User.UserId ? expense.Cost : 0;
					result[index].Payables.Add(transactions[index]);

					index++;
				}

				//var receivables = new List<Transaction>();

				//Each
				foreach (var transaction in transactions)
				{
					result[expenseAllocatedUserIndex].Receivables.Add(transaction);
				}

				//result[expenseAllocatedUserIndex].Receivables = new ObservableCollection<Transaction>(receivables);

				initial = false;
				index = 0;
			}

			return result;
		}

		public static IEnumerable<UserExpenseAggregate> CalculateHostManagedTransaction(Event evnt)
		{
			var expenses = evnt.Expenses;

			if (expenses == null)
			{
				if (evnt?.Guests == null) return null;
				return ExpenseHelper.GetUserExpenseAggregate(evnt.Guests);
			}

			var result = new List<UserExpenseAggregate>();
			bool initial = true;
			int index = 0;

			foreach (var expense in expenses)
			{
				var assignedUser = expense.CostAllocations.FirstOrDefault(ca =>
						ca.User.UserId == expense.AssignedUser.UserId);

				if (assignedUser == null)
					return null;

				IList<Transaction> transactions = ExpenseHelper.CalculateTransactions(expense).ToList();
				var expenseAllocatedUser = expense.CostAllocations.FirstOrDefault(c => c.UserRef == expense.UserRef);
				var expenseAllocatedUserIndex = expense.CostAllocations.IndexOf(expenseAllocatedUser);

				foreach (var costAllocation in expense.CostAllocations)
				{
					if (initial)
					{
						var uee = new UserExpenseAggregate();
						uee.User = costAllocation.User;
						uee.Cost = costAllocation.AllocatedCost;

						result.Add(uee);
					}
					else
					{
						result[index].Cost += costAllocation.AllocatedCost;
					}

					result[index].Spend += result[index].User.UserId == assignedUser.User.UserId ? expense.Cost : 0;
					result[index].Payables.Add(transactions[index]);

					index++;
				}

				foreach (var transaction in transactions)
				{
					result[expenseAllocatedUserIndex].Receivables.Add(transaction);
				}


				initial = false;
				index = 0;
			}

			return result;
		}

		public static IEnumerable<UserExpenseAggregate> CalculateMinimumTransaction(Event evnt)
		{
			var expenses = evnt.Expenses;

			if (expenses == null)
			{
				if (evnt?.Guests == null) return null;
				return ExpenseHelper.GetUserExpenseAggregate(evnt.Guests);
			}

			var result = new List<UserExpenseAggregate>();
			bool initial = true;
			int index = 0;

			foreach (var expense in expenses)
			{
				var assignedUser = expense.CostAllocations.FirstOrDefault(ca =>
						ca.User.UserId == expense.AssignedUser.UserId);

				if (assignedUser == null)
					return null;

				IList<Transaction> transactions = ExpenseHelper.CalculateTransactions(expense).ToList();
				var expenseAllocatedUser = expense.CostAllocations.FirstOrDefault(c => c.UserRef == expense.UserRef);
				var expenseAllocatedUserIndex = expense.CostAllocations.IndexOf(expenseAllocatedUser);

				foreach (var costAllocation in expense.CostAllocations)
				{
					if (initial)
					{
						var uee = new UserExpenseAggregate();
						uee.User = costAllocation.User;
						uee.Cost = costAllocation.AllocatedCost;

						result.Add(uee);
					}
					else
					{
						result[index].Cost += costAllocation.AllocatedCost;
					}

					result[index].Spend += result[index].User.UserId == assignedUser.User.UserId ? expense.Cost : 0;
					result[index].Payables.Add(transactions[index]);

					index++;
				}

				foreach (var transaction in transactions)
				{
					result[expenseAllocatedUserIndex].Receivables.Add(transaction);
				}

				initial = false;
				index = 0;
			}

			return result;
		}

	}
}
