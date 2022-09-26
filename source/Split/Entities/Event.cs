using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Split.Entities
{
	public class Event
	{
		public int EventId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public TransactionStrategy TransactionStrategy { get; set; }

		public ICollection<User> Hosts { get; set; }
		public ICollection<User> Guests { get; set; }
		public ICollection<Expense> Expenses { get; set; }

		public ICollection<EventUser> EventUsers { get; set; }
	}
}
