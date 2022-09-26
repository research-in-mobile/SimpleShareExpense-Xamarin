using System;
using System.Collections.Generic;
using System.Text;

namespace Split.Entities
{
	public class EventUser
	{
		public int EventRef { get; set; }
		public Event Event { get; set; }

		public int UserRef { get; set; }
		public User User { get; set; }
	}
}
