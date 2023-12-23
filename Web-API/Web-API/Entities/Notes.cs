using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_API.Entities
{
	public class Notes
	{
        [Key]
        public int ID { get; set; }

        public int StudentNotesID { get; set; }

        public int CourseID { get; set; }

        public int Score { get; set; }

        public string? LetterScore { get; set; } // Harici sabit bir tabloya al.

        public bool IsActive { get; set; }
    }
}

