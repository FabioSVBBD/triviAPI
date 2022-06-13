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
					where d.DifficultyName.ToLower() == difficultyName.ToLower()
					select d).ToList().FirstOrDefault();
		}

		public Category? getCategoryObject(string categoryName)
		{
			return (from c in _context.Categories
					where c.CategoryName.ToLower() == categoryName.ToLower()
					select c).ToList().FirstOrDefault();
		}

		public Status? getStatusByObject(string statusName)
		{
			return (from s in _context.Statuses
					where s.StatusName.ToLower() == statusName.ToLower()
					select s).ToList().FirstOrDefault();
		}

		public Tag? getTagObject(string tagName)
        {
			return (from t in _context.Tags
					where t.TagName.ToLower() == tagName.ToLower()
					select t).ToList().FirstOrDefault();
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

		public Dictionary<String, String> getPendingFailResponse()
		{
			return new() { { "message", "An issue occurred setting status to pending. We have some maintenance to do." } };
		}

		public Dictionary<String, String> getDeletedFailResponse()
		{
			return new() { { "message", "An issue occurred setting setting status to deleted. We have some maintenance to do." } };
		}
	}
}
