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

			return Ok(_context.Questions.Find(id));
		}

		[HttpPatch("{id}")]
		public IActionResult updateQuestion(int id, [FromBody] QuestionDTO questionPatches)
		{
			Question? questionToChange = _context.Questions.Find(id);

			if (questionToChange == null)
			{
				return NotFound();
			}

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
				int difficultyForPatchId = _valueToIdUtil.getDifficulty(questionPatches.Difficulty);
				questionToChange.DifficultyId = questionToChange.Difficulty.DifficultyId = difficultyForPatchId;
				questionToChange.Difficulty.DifficultyName = questionPatches.Difficulty;
			}

			if (!string.IsNullOrEmpty(questionPatches.Category))
			{
				int categoryId = _valueToIdUtil.getCategory(questionPatches.Category);
				questionToChange.CategoryId = questionToChange.Category.CategoryId = categoryId;
				questionToChange.Category.CategoryName = questionPatches.Category;
			}

			if (!string.IsNullOrEmpty(questionPatches.Status))
			{
				int statusId = _valueToIdUtil.getStatus(questionPatches.Status);
				questionToChange.StatusId = questionToChange.Status.StatusId = statusId;
				questionToChange.Status.StatusName = questionPatches.Status;
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
			Question? question = _context.Questions.Find(id);

			if (question == null)
			{
				return NotFound();
			}

			if (string.IsNullOrEmpty(updatedQuestion.Question)||
				string.IsNullOrEmpty(updatedQuestion.Answer) ||
				string.IsNullOrEmpty(updatedQuestion.Difficulty) ||
				string.IsNullOrEmpty(updatedQuestion.Category) ||
				string.IsNullOrEmpty(updatedQuestion.Status))
			{
				return BadRequest("You are missing some fields");
			}

			question.Question1 = updatedQuestion.Question;
			question.Answer = updatedQuestion.Answer;

			int difficultyID = _valueToIdUtil.getDifficulty(updatedQuestion.Difficulty);
			question.Difficulty = new Difficulty();

			question.DifficultyId = question.Difficulty.DifficultyId = difficultyID;
			question.Difficulty.DifficultyName = updatedQuestion.Difficulty;

			int categoryId = _valueToIdUtil.getCategory(updatedQuestion.Category);
			question.Category = new Category();

			question.CategoryId = question.Category.CategoryId = categoryId;
			question.Category.CategoryName = updatedQuestion.Category;

			int statusId = _valueToIdUtil.getStatus(updatedQuestion.Status);
			question.Status = new Status();

			question.StatusId = question.Status.StatusId = statusId;
			question.Status.StatusName = updatedQuestion.Status;

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
