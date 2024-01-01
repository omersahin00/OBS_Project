using System;
namespace App.Managers
{
	public static class LetterScoreManager
	{

		public static string LetterScoreCalculator(int Score)
		{
            if (Score <= 100 && Score >= 82) return "AA";
            else if (Score <= 81 && Score >= 74) return "BA";
            else if (Score <= 73 && Score >= 65) return "BB";
            else if (Score <= 64 && Score >= 58) return "CB";
            else if (Score <= 57 && Score >= 50) return "CC";
            else if (Score <= 49 && Score >= 40) return "DC";
            else if (Score <= 39 && Score >= 35) return "DD";
            else if (Score <= 34 && Score >= 25) return "FD";
            else if (Score <= 24 && Score >= 0) return "FF";
            else return "NULL";
        }
	}
}

