using System;
using System.Collections.Generic;
using System.Text;

namespace Split.Entities
{
	public class UserEventExpense
	{
		public int UserEventExpenseId { get; set; }
		public double Spend { get; set; }
		public double Cost { get; set; }
		public double Balance { get; set; }
		public bool IsResolved { get; set; }

		public int UserRef { get; set; }
		public User User { get; set; }

		public bool IsComplete { get; set; }
		public ICollection<Transaction> Transactions { get; set; }
	}
}
