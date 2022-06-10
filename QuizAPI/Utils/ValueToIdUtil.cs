using QuizAPI.Model;
using System.Linq;

namespace QuizAPI.Utils
{
	public class ValueToIdUtil
	{
		TriviapiDBContext _context = new TriviapiDBContext();

		public int getDifficulty(string difficultyName)
		{
			Difficulty? difficultyToGet = (from d in _context.Difficulties
																		 where d.DifficultyName == difficultyName
																		 select d).ToList().FirstOrDefault();

			return difficultyToGet == null ? -1 : difficultyToGet.DifficultyId;
		}

		public int getCategory(string categoryName)
		{
			Category? categoryToGet = (from c in _context.Categories
																 where c.CategoryName == categoryName
																 select c).ToList().FirstOrDefault();

			return categoryToGet == null ? -1 : categoryToGet.CategoryId;
		}

		public int getStatus(string statusName)
		{
			Status? statusToGet = (from s in _context.Statuses
														 where s.StatusName == statusName
														 select s).ToList().FirstOrDefault();

			return statusToGet == null ? -1 : statusToGet.StatusId;
		}
	}
}
