using QuizAPI.Model;
using System.Linq;

namespace QuizAPI.Utils
{
	public class ValueToIdUtil
	{
		TriviapiDBContext _context = new TriviapiDBContext();

		public bool questionExists(int id)
		{
			return _context.Questions.Find(id) != null;
		}

		public Difficulty? getDifficultyObject(string difficultyName)
		{
			return (from d in _context.Difficulties
							where d.DifficultyName == difficultyName
							select d).ToList().FirstOrDefault();
		}

		public Category? getCategoryObject(string categoryName)
		{
			return (from c in _context.Categories
							where c.CategoryName == categoryName
							select c).ToList().FirstOrDefault();
		}

		public Status? getStatusByObject(string statusName)
		{
			return (from s in _context.Statuses
							where s.StatusName == statusName
							select s).ToList().FirstOrDefault();
		}

		public Dictionary<String, List<String>> getInvalidDifficultyResponse()
		{
			return new() {
				{ "message", new List<string>() { "Invalid Difficulty" } },
				{ "values", _context.Difficulties.ToList().Select(d => d.DifficultyName).ToList() }
			};
		}

		public Dictionary<String, List<String>> getInvalidCategoryResponse()
		{
			return new() {
				{ "message", new List<string>() { "Invalid Category" } },
				{ "values", _context.Categories.ToList().Select(c => c.CategoryName).ToList() }
			};
		}

		public Dictionary<String, List<String>> getInvalidStatusResponse()
		{
			return new() {
				{ "message", new List<string>() { "Invalid Status" } },
				{ "values", _context.Statuses.ToList().Select(s => s.StatusName).ToList() }
			};
		}

		public Dictionary<String, List<String>> getInvalidTagResponse()
		{
			return new() {
				{ "message", new List<string>() { "Invalid Tag" } },
				{ "values", _context.Tags.ToList().Select(t => t.TagName).ToList() }
			};
		}
	}
}
