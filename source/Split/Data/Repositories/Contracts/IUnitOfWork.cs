using Split.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Split.Data
{
	public interface IUnitOfWork : IDisposable
	{
		UserRepository Users { get; }
		EventRepository Events { get; }
		ExpenseRepository Expenses { get; }
		IRepository<Contact> Contacts { get; }
		IRepository<UserEventExpense> UserEventExpenses { get; }
		IRepository<EventUser> EventUsers { get; }
		IRepository<Transaction> Payments { get; }
		IRepository<CostAllocation> CostAllocations { get; }
		//IRepository<AppSettings> AppSettings { get;}

		AppDbContext GetCurrentContext();

		int Complete();
	}
}
