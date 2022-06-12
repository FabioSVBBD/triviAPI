using Microsoft.AspNetCore.Mvc;
using QuizAPI.Model;
using QuizAPI.Utils;
using QuizAPI.DTOs;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace QuizAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class QuizController : ControllerBase
	{
		TriviapiDBContext _context = new TriviapiDBContext();
		ValueToIdUtil _valueToIdUtil = new ValueToIdUtil();

		[HttpGet("{id}")]
		public IActionResult getQuestionById(int id)
		{
			var question = _context.Questions
		.Where(question => question.QuestionId == id)
		.Include(question => question.Category)
		.Include(question => question.Difficulty)
		.Include(question => question.Status);

			if (question == null)
			{
				return NotFound();
			}

			return Ok(question);
		}

		[HttpGet]
		public IActionResult getAllQuestions()
		{
			var questions = _context.Questions.ToList();

			return Ok(questions);
		}

		[HttpGet("categories")]
		public IActionResult getallCategories()
		{
			var categories = _context.Categories.ToList();


			if (categories == null)
			{
				return NotFound();
			}
			return Ok(categories);
		}

        [HttpGet("statuses")]
		public IActionResult getAllStatuses()
        {
			var statuses = _context.Statuses.ToList();


			if (statuses == null)
            {
				return NotFound();
            }
			return Ok(statuses);
        }

		[HttpGet("categories/CategoryName")]
		public IActionResult getQuestionsbyName(string categoryName)
		{
			var categories = _context.Categories
				.Where(name => name.CategoryName == categoryName)
				.Include(questions => questions.Questions);

			if (categories == null)
			{
				return NotFound();
			}
			return Ok(categories);
		}


		[HttpGet("difficulty/level")]
		public IActionResult getDifficult(string level)
		{
			var difficults = _context.Difficulties
				.Where(difficult => difficult.DifficultyName == level)
				.Include(question => question.Questions);

			if (difficults == null)
			{
				return NotFound();
			}
			return Ok(difficults);
		}

		[HttpGet("Status/statusCode")]
		public IActionResult getQuestionsByStatusCode(string statusCode)
        {
			var status = _context.Statuses
				.Where(statusTbl => statusTbl.StatusName == statusCode)
				.Include(questionTbl => questionTbl.Questions);
			if (status == null)
            {
				return NotFound();
            }
			return Ok(status);
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

            var newQuestion = new Question();
            newQuestion.Question1 = newQuestionDetails.Question;
            newQuestion.Answer = newQuestionDetails.Answer;


            var category = _valueToIdUtil.getCategoryObject(newQuestionDetails.Category);
            if (category == null)
            {
                return BadRequest("Category does not exist");
            }

            newQuestion.Category = category;
            newQuestion.CategoryId = category.CategoryId;


            var difficulty = _valueToIdUtil.getDifficultyObject(newQuestionDetails.Difficulty);
            if (difficulty == null)
            {
                return BadRequest("Difficulty does not exist");
            }
            newQuestion.Difficulty = difficulty;
            newQuestion.DifficultyId = difficulty.DifficultyId;


            var status = _valueToIdUtil.getStatusObject(newQuestionDetails.Status);
            if (status == null)
            {
                return BadRequest("Status does not exist");
            }
            newQuestion.Status = status;
            newQuestion.StatusId = status.StatusId;

            try
            {
                _context.Questions.Update(newQuestion);
                _context.SaveChanges();

                return Ok(QuestionDTO.AsDTO(newQuestion));
            }
            catch (Exception e)
            {
                _ = e;
                return BadRequest("Failed To Connect to Database"); ;
            }


        }




    }
}
