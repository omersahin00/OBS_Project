using System;
using System.ComponentModel.DataAnnotations;

namespace Web_API.Entities
{
	public class Course
	{
        [Key]
        public int ID { get; set; }

        public string? Name { get; set; }

        public int Credit { get; set; }

        public bool IsUsed { get; set; }

        public bool IsActive { get; set; }
    }
}

