using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Split.Data;
using Split.Entities;
using Split.Helpers;
using Split.Models;

namespace Split.Services
{
	public class EventService : IEventService
	{
		private readonly IUnitOfWork _uow;
		private readonly ILogService _logService;

		public EventService(IUnitOfWork uow,
							ILogService logService)
		{
			_uow = uow;
			_logService = logService;
		}

		private int SaveChanges() => _uow.Complete();

		public async Task<IEnumerable<Event>> GetEventsAsync() => await _uow.Events.GetAllAsync();

		public async Task<Event> GetEventAsync(int id) => await _uow.Events.GetAsync(id);

		public async Task<Event> AddEventAsync(Event evnt)
		{
			await _uow.Events.AddAsync(evnt);

			var savedChanges = SaveChanges();

			return savedChanges > 0 ? evnt : null;
		}

		public async Task<bool> DeleteEventAsync(Event evnt)
		{
			await _uow.Events.RemoveAsync(evnt.EventId);
			var savedChanges = SaveChanges();

			return savedChanges > 0;
		}

		public async Task<Event> UpdateEventAsync(Event newEvent)
		{
			SaveChanges();

			return newEvent;
		}

		public double TotalExpense(Event evnt)
		{
			double total = 0;

			if (evnt?.Expenses == null)
				return total;

			foreach (var expense in evnt.Expenses)
			{
				total += expense.Cost;
			}

			return total;
		}

		public int NumberOfGuests(Event evnt)
		{
			return evnt?.Guests != null ? evnt.Guests.Count : 0;
		}

		public async Task<IEnumerable<Expense>> GetEventExpensesAsync(Event evnt)
		{
			return await _uow.Expenses.GetExpensesFromEventAsync(evnt.EventId);
		}

		public async Task<IEnumerable<IEnumerable<CostAllocation>>> GetEventCostAllocationsAsync(Event evnt)
		{
			var expenses = await GetEventExpensesAsync(evnt);

			return expenses.Select(e => e.CostAllocations);
		}

		public async Task<int> GetNumberOfGuests(Event evnt)
		{
			var expenses = await GetEventExpensesAsync(evnt);

			return expenses.Select(e => e.CostAllocations).Count();
		}

		public Dictionary<string, TransactionStrategy> GetTransactionStrategies()
		{
			//TODO: check if Network is available and call api if no network get local

			return EventHelper.TransactionStrategies;
		}
	}
}
