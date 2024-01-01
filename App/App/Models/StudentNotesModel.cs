using System;

namespace App.Models
{
	public class StudentNotesModel
	{
        public int NoteID { get; set; }

        public string? Name { get; set; }

        public int Credit { get; set; }

        public int Score { get; set; }

        public string? LetterScore { get; set; }

        public bool IsActive { get; set; }
    }
}

