using Microsoft.AspNetCore.Mvc;
using QuizAPI.Model;
using QuizAPI.Utils;
using QuizAPI.DTOs;
using System.Text.Json;

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

		[HttpGet]
		public IActionResult getAllQuestions()
		{
			var questions = _context.Questions.ToList();  


			return Ok(questions);
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


		[HttpPost]
		public IActionResult insertQuestion([FromBody] QuestionDTO newQuestionDetails)
		{


			if (string.IsNullOrEmpty(newQuestionDetails.Question) ||
				string.IsNullOrEmpty(newQuestionDetails.Answer) ||
				string.IsNullOrEmpty(newQuestionDetails.Difficulty) ||
				string.IsNullOrEmpty(newQuestionDetails.Category) ||
				string.IsNullOrEmpty(newQuestionDetails.Status))
			{
				return BadRequest("You are missing some fields");
			}

			//Make new question object
			var newQuestion = new Question();
			newQuestion.Question1 = newQuestionDetails.Question;
			newQuestion.Answer = newQuestionDetails.Answer;


			var category = _valueToIdUtil.getCategoryObject(newQuestionDetails.Category);
			newQuestion.Category = category;
			newQuestion.CategoryId = category.CategoryId;


			var difficulty = _valueToIdUtil.getDifficultyObject(newQuestionDetails.Difficulty);
			newQuestion.Difficulty = difficulty;
			newQuestion.DifficultyId = difficulty.DifficultyId;

			var status = _valueToIdUtil.getStatusObject(newQuestionDetails.Status);
			newQuestion.Status = status;
			newQuestion.StatusId = status.StatusId;




			Console.WriteLine(JsonSerializer.Serialize(newQuestion));
			try
			{
				_context.Questions.Add(newQuestion);
				_context.SaveChanges();

				return Accepted( QuestionDTO.AsDTO(newQuestion));
			}
			catch (Exception e)
			{
				_ = e;
				Console.WriteLine(e);
				return BadRequest("Failed To Connect to Database"); ;
			}


		}



	}
}
