using System;
using System.Collections.Generic;
using System.Text;

namespace Split.Entities
{
	public class CostAllocation
	{
		public int CostAllocationId { get; set; }
		public bool IsSplitWith { get; set; }
		public double AllocationPercentage { get; set; }
		public int AllocationWeight { get; set; }
		public double AllocatedCost { get; set; }

		public int UserRef { get; set; }
		public User User { get; set; }

		public int ExpenseRef { get; set; }
		public Expense Expense { get; set; }
	}
}
