using Split.Entities;
using Split.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Split.Services
{
	public interface IEventService
	{
		Task<IEnumerable<Event>> GetEventsAsync();
		Task<Event> GetEventAsync(int id);
		Task<Event> AddEventAsync(Event evnt);
		Task<bool> DeleteEventAsync(Event evnt);
		Task<Event> UpdateEventAsync(Event evnt);

		double TotalExpense(Event evnt);
		int NumberOfGuests(Event evnt);

		Task<IEnumerable<Expense>> GetEventExpensesAsync(Event evnt);
		Task<IEnumerable<IEnumerable<CostAllocation>>> GetEventCostAllocationsAsync(Event evnt);
		Task<int> GetNumberOfGuests(Event evnt);

		Dictionary<string, TransactionStrategy> GetTransactionStrategies();

	}
}