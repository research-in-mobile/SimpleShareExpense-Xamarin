using System;
using System.Collections.Generic;
using System.Text;

namespace Split.Entities
{
    public class Contact
    {
		public int  ContactId { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string AddressLine1 { get; set; }
		public string AddressLine2 { get; set; }

		public int UserRef { get; set; }
		public User User { get; set; }
    }
}
