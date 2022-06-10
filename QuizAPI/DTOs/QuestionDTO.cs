namespace QuizAPI.DTOs
{
	public class QuestionDTO
	{
		private string? question;
		private string? answer;
		private string? difficulty;
		private string? category;
		private string? status;

		public string? Status { get => status; set => status = value; }
		public string? Category { get => category; set => category = value; }
		public string? Difficulty { get => difficulty; set => difficulty = value; }
		public string? Answer { get => answer; set => answer = value; }
		public string? Question { get => question; set => question = value; }
	}
}
