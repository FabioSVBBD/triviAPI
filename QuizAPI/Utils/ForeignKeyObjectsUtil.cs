﻿using QuizAPI.Model;
using System.Linq;
using QuizAPI.DTOs;

namespace QuizAPI.Utils
{
	public class ForeignKeyObjectsUtil
	{

		TriviapiDBContext _context = new TriviapiDBContext();

		public bool questionExists(int id)
		{
			return _context.Questions.Find(id) != null;
		}

		public Difficulty? getDifficulty(string difficultyName)
		{
			return (from d in _context.Difficulties
					where d.DifficultyName.ToLower() == difficultyName.ToLower()
					select d).ToList().FirstOrDefault();
		}

		public Category? getCategory(string categoryName)
		{
			return (from c in _context.Categories
					where c.CategoryName.ToLower() == categoryName.ToLower()
					select c).ToList().FirstOrDefault();
		}

		public Tag? getTag(string tagName)
        {
			return (from t in _context.Tags
					where t.TagName.ToLower() == tagName.ToLower()
					select t).ToList().FirstOrDefault();
		}

		public List<string> getTagsForQuestion(int questionId)
        {
			return (from t in _context.Tags
					join qt in _context.QuestionTags
					on t.TagId equals qt.TagId
					where qt.QuestionId == questionId
					select new
					{
						tagName = t.TagName
					}).ToList().Select(t => t.tagName).ToList();
		}

		public Status? getStatus(string statusName)
		{
			return (from s in _context.Statuses
					where s.StatusName.ToLower() == statusName.ToLower()
					select s).ToList().FirstOrDefault();

		}
	}
}
