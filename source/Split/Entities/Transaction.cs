using System;
using System.Collections.Generic;
using System.Text;

namespace Split.Entities
{
	public class Transaction
	{
		public int TransactionId { get; set; }
		public double Amount { get; set; }
		public bool IsComplete { get; set; }
		public bool IsAutoComplete { get; set; }


		public int PayableUserRef { get; set; }
		public User PayableUser { get; set; }

		public int ReceivableUserRef { get; set; }
		public User ReceivableUser { get; set; }

		public int ExpenseRef { get; set; }
		public Expense Expense { get; set; }

		public int UserEventExpenseRef { get; set; }
		public UserEventExpense UserEventExpense { get; set; }
	}
}
