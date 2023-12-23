using System;
using System.ComponentModel.DataAnnotations;

namespace Web_API.Entities
{
	public class Employee
	{
		[Key]
		public int ID { get; set; }

		public int UserNumber { get; set; }

		public string? FirstName { get; set; }

		public string? LastName { get; set; }

		public string? Password { get; set; }

		public bool IsActive { get; set; }
	}
}

