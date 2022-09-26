using Split.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections;

namespace Split.Data
{
	public class EventRepository : Repository<Event>, IEventRepository
	{
		public EventRepository(AppDbContext context) : base(context)
		{

		}

		public async Task<IEnumerable<Event>>GetAllEventsFull()
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<Event>> GetEventFull(int eventId)
		{
			var result = await Context.Events
								.Where(e => e.EventId == eventId)
								.Include(e => e.Expenses)
								.ToListAsync();

			return result;
		}
	}
}
