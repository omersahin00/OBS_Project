using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_API.Entities
{
	public class Student : Account
	{
        [Key]
        public int ID { get; set; }

        public string? Number { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        // ID ile bire bir aynı olacak:
        public int StudentNotesID { get; set; }

        public string? Password { get; set; }

        public bool IsActive { get; set; }
    }
}

