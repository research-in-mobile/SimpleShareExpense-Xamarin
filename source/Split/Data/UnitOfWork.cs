using Microsoft.EntityFrameworkCore;
using Split.Entities;
using Split.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split.Data
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly AppDbContext _context;

		public UnitOfWork(AppDbContext context)
		{
			_context = context;

			
			Events = new EventRepository(_context);
			Expenses = new ExpenseRepository(_context);
			Users = new UserRepository(_context);

			Contacts = new Repository<Contact>(_context);
			UserEventExpenses = new Repository<UserEventExpense>(_context);
			EventUsers = new Repository<EventUser>(_context);		
			Payments = new Repository<Transaction>(_context);
			CostAllocations = new Repository<CostAllocation>(_context);

			//AppSettings = new Repository<AppSettings>(_context);

			Task.Run(async () =>
			{
				await LoadRepositories();
				EventHelper.OnDataContextChanged(this, new EventArgs());
			});
		}

		public IRepository<Contact> Contacts { get; private set; }
		public IRepository<UserEventExpense> UserEventExpenses { get; private set; }
		public UserRepository Users { get; private set; }
		public EventRepository Events { get; private set; }
		public IRepository<EventUser> EventUsers { get; private set; }
		public ExpenseRepository Expenses { get; private set; }
		public IRepository<Transaction> Payments { get; private set; }
		public IRepository<CostAllocation> CostAllocations { get; private set; }
		//public IRepository<AppSettings> AppSettings { get; private set; }

		public AppDbContext GetCurrentContext()
		{
			return this._context;
		}

		protected async Task LoadRepositories()
		{
			//var appSettings = await AppSettings.GetAllAsync();
			var EventsLoadTask = await Events.GetAllAsync();
			var UsersTask = await Users.GetAllAsync();

			var ContactsTask = await Contacts.GetAllAsync();
			var UEETask = await UserEventExpenses.GetAllAsync();
			var EUTask = await EventUsers.GetAllAsync();

			var ExpensesTask = await Expenses.GetAllAsync();
			var PaymentsTask = await Payments.GetAllAsync();
			var CATask = await CostAllocations.GetAllAsync();
		}

		public int Complete()
		{
			return _context.SaveChanges();
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
