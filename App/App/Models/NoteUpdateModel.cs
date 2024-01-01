using System;
namespace App.Models
{
	public class NoteUpdateModel
	{
		public int NoteID { get; set; }

		public string? StudentNumber { get; set; }

		public string? FirstName { get; set; }

		public string? LastName { get; set; }

		public int Score { get; set; }

		public string? LetterScore { get; set; }
	}
}

