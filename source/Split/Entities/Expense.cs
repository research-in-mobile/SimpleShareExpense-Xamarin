using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Split.Entities
{
	public class Expense
	{
		public int ExpenseId { get; set; }
		public string Title { get; set; }
		public double Cost { get; set; }
		public string CurrencyType { get; set; }
		public double ConvertaionRate { get; set; }
		public double ConvertedCost { get; set; }
		public DateTime ExpenseDate { get; set; }
		public bool IsPaid { get; set; }
		public IList<CostAllocation> CostAllocations { get; set; }

		public int UserRef { get; set; }
		public User AssignedUser { get; set; }

		public int EventRef { get; set; }
		public Event Event { get; set; }

	}
}
