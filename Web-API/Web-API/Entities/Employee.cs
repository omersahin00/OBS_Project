using System;
using System.ComponentModel.DataAnnotations;

namespace Web_API.Entities
{
	public class Employee : Account
	{
		[Key]
		public int ID { get; set; }

		public string? UserNumber { get; set; }

		public string? FirstName { get; set; }

		public string? LastName { get; set; }

		public string? Password { get; set; }

		public bool IsActive { get; set; }
	}
}

