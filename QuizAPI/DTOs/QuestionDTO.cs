using QuizAPI.Model;

namespace QuizAPI.DTOs
{
	public class QuestionDTO
	{
		private string? question;
		private string? answer;
		private string? difficulty;
		private string? category;
		private string? status;
		private string[]? tags;

		public QuestionDTO(String question, String answer, String status, String difficulty, String category, String[]? tags)
		{
			this.question = question;
			this.answer = answer;
			this.status = status;
			this.difficulty = difficulty;
			this.category = category;
			this.tags = tags; 
		}

		public string? Status { get => status; set => status = value; }
		public string? Category { get => category; set => category = value; }
		public string? Difficulty { get => difficulty; set => difficulty = value; }
		public string? Answer { get => answer; set => answer = value; }
		public string? Question { get => question; set => question = value; }
        public string[]? Tags { get => tags; set => tags = value; }

		public static QuestionDTO? AsDTO(Question? question, String[]? tags )
		{
			if (question == null) {
				return null;
			}

			return new QuestionDTO(question.Question1, question.Answer, question.Status.StatusName, question.Difficulty.DifficultyName, question.Category.CategoryName, tags );
		}
	}
}
