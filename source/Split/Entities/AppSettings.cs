using System;
using System.Collections.Generic;
using System.Text;

namespace Split.Entities
{
	public class AppSettings
	{
		public int AppSettingsId { get; set; }

		public string DefaultCurrency { get; set; }

		public int UserRef { get; set; }
		public User User { get; set; } 

	}
}
