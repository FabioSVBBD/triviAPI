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

	}
}
