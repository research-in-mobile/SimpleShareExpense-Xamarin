using System;
using System.Collections.Generic;
using System.Text;

namespace Split.Entities
{
	public class User
	{
		protected User()
		{
			GUID = Guid.NewGuid();
		}

		public User(string name, UserEntityType entityType = UserEntityType.Person)
		{
			GUID = Guid.NewGuid();
			Name = name;
			Type = entityType;
		}

		public User(User user)
		{
			GUID = user.GUID;
			Name = user.Name;
			Type = user.Type;

			Contact = user.Contact;
			UserEventExpenses = user.UserEventExpenses;
			EventUsers = user.EventUsers;
			Payables = user.Payables;
			Receivables = user.Receivables;
			CostAllocations = user.CostAllocations;
		}

		public int UserId { get; set; }
		public Guid GUID { get; private set; }
		public string Name { get; set; }
		public UserEntityType Type { get; set; }

		public Contact Contact { get; set; }

		public ICollection<UserEventExpense> UserEventExpenses { get; set; }
		public ICollection<Expense> UserExpenses { get; set; }
		public ICollection<EventUser> EventUsers { get; set; }
		public ICollection<Transaction> Payables { get; set; }
		public ICollection<Transaction> Receivables { get; set; }
		public ICollection<CostAllocation> CostAllocations { get; set; }

		//public AppSettings AppSettings { get; set; }


	}
}
