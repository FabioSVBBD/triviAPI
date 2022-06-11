using Microsoft.AspNetCore.Mvc;
using QuizAPI.Model;
using QuizAPI.Utils;
using QuizAPI.DTOs;

namespace QuizAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class QuizController : ControllerBase
	{
		TriviapiDBContext _context = new TriviapiDBContext();
		ValueToIdUtil _valueToIdUtil = new ValueToIdUtil();

		[HttpGet("{id}")]
		public IActionResult testMe(int id)
		{
			Question? question = _context.Questions.Find(id);

			if (question == null)
			{
				return NotFound();
			}

			return Ok(question);
		}

		[HttpPatch("{id}")]
		public IActionResult updateQuestion(int id, [FromBody] QuestionDTO questionPatches)
		{
			if (!_valueToIdUtil.questionExists(id))
			{
				return NotFound();
			}

			Question? questionToChange = _context.Questions.Find(id);

			if (!string.IsNullOrEmpty(questionPatches.Question))
			{
				questionToChange.Question1 = questionPatches.Question;
			}

			if (!string.IsNullOrEmpty(questionPatches.Answer))
			{
				questionToChange.Answer = questionPatches.Answer;
			}

			if (!string.IsNullOrEmpty(questionPatches.Difficulty))
			{
				questionToChange.Difficulty = _valueToIdUtil.getDifficultyObject(questionPatches.Difficulty);
				questionToChange.DifficultyId = questionToChange.Difficulty.DifficultyId;
			}

			if (!string.IsNullOrEmpty(questionPatches.Category))
			{
				questionToChange.Category = _valueToIdUtil.getCategoryObject(questionPatches.Category);
				questionToChange.CategoryId = questionToChange.Category.CategoryId;
			}

			if (!string.IsNullOrEmpty(questionPatches.Status))
			{
				questionToChange.Status = _valueToIdUtil.getStatusByObject(questionPatches.Status);
				questionToChange.StatusId = questionToChange.Status.StatusId;
			}

			try
			{
				_context.Questions.Update(questionToChange);
				_context.SaveChanges();

				return Ok(_context.Questions.Find(id));
			}
			catch (Exception e)
			{
				_ = e;
				return BadRequest("Invalid Values");
			}
		}

		[HttpPut("{id}")]
		public IActionResult putQuestion(int id, [FromBody] QuestionDTO updatedQuestion)
		{
			if (!_valueToIdUtil.questionExists(id))
			{
				return NotFound();
			}

			Question? question = _context.Questions.Find(id);

			if (string.IsNullOrEmpty(updatedQuestion.Question) ||
				string.IsNullOrEmpty(updatedQuestion.Answer) ||
				string.IsNullOrEmpty(updatedQuestion.Difficulty) ||
				string.IsNullOrEmpty(updatedQuestion.Category) ||
				string.IsNullOrEmpty(updatedQuestion.Status))
			{
				return BadRequest("You are missing some fields");
			}

			question.Question1 = updatedQuestion.Question;
			question.Answer = updatedQuestion.Answer;

			if (question.Difficulty.DifficultyName != updatedQuestion.Difficulty)
			{
				Difficulty? fetchedDifficulty = _valueToIdUtil.getDifficultyObject(updatedQuestion.Difficulty);

				if (fetchedDifficulty == null)
				{
					return BadRequest(_valueToIdUtil.getInvalidDifficultyResponse());
				}

				question.Difficulty = fetchedDifficulty;
				question.DifficultyId = question.Difficulty.DifficultyId;
			}

			if (question.Category.CategoryName != updatedQuestion.Category)
			{
				Category? fetchedCategory = _valueToIdUtil.getCategoryObject(updatedQuestion.Category);

				if (fetchedCategory == null)
				{
					return BadRequest(_valueToIdUtil.getInvalidCategoryResponse());
				}

				question.Category = fetchedCategory;
				question.CategoryId = question.Category.CategoryId;
			}

			if (question.Status.StatusName != updatedQuestion.Status)
			{
				Status? fetchedStatus = _valueToIdUtil.getStatusByObject(updatedQuestion.Status);

				if (fetchedStatus == null)
				{
					return BadRequest(_valueToIdUtil.getInvalidStatusResponse());
				}

				question.Status = fetchedStatus;
				question.StatusId = question.Status.StatusId;
			}

			try
			{
				_context.Questions.Update(question);
				_context.SaveChanges();

				return Ok(_context.Questions.Find(id));
			}
			catch (Exception e)
			{
				_ = e;
				return BadRequest("Failed To Connect to Database");
			}
		}
	}
}
