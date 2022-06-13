﻿using Microsoft.AspNetCore.Mvc;
using QuizAPI.Model;
using QuizAPI.Utils;
using QuizAPI.Utilities;
using Microsoft.EntityFrameworkCore;
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
		public IActionResult getAllQuestions([FromQuery] QueryParam parameters)
		{
            parameters.baseUrl = Request.Host.Value + Request.Path;
			PaginationHandler _page = new PaginationHandler(_context.Categories, _context.Difficulties);

			var questions = _context.Questions;
			var paginatedBody = _page.paginateQuestions(questions, parameters);

			return paginatedBody.results.Count > 0 ? Ok(paginatedBody) : NotFound(paginatedBody);
		}


        [HttpGet("difficulties")]
		public IActionResult getAllDifficulties()
        {
			var difficulties = _context.Difficulties.Select(diff => diff.DifficultyName );

			if (difficulties == null)
            {
				return NotFound();
            }

			return Ok(difficulties);
        }

		[HttpGet("categories")]
		public IActionResult getallCategories()
		{
			var categories = _context.Categories.Select(c => c.CategoryName);

			if (categories == null)
            {
				return NotFound();
            }

			return Ok(categories);
		}
		
        [HttpGet("statuses")]
		public IActionResult getAllStatuses()
        {
			var statuses = _context.Statuses.Select(status => status.StatusName);


			if (statuses == null)
            {
				return NotFound();
            }
			return Ok(statuses);
        }

		[HttpGet("tags")]
		public IActionResult getAllTags()
        {
			var tags = _context.Tags.Select(tag => tag.TagName);

			if (tags == null)
            {
				return NotFound();
            }
			return Ok(tags);
        }

        [HttpGet("tags/TagName")]
		public IActionResult GetAllQuestionsByName(string TagName)
        {
			var tagID = _context.Tags.Where(tagTbl => tagTbl.TagName == TagName).Select(id => id.TagId).First();

			var questionID = _context.QuestionTags.Where(qtagTbl => qtagTbl.TagId == tagID).Select(id => id.QuestionId).First();

			var questions = _context.Questions.Where(tagTbl => tagTbl.QuestionId == questionID);

			PaginationHandler pg = new PaginationHandler(_context.Categories, _context.Difficulties);

			return Ok(pg.paginateQuestions(questions, new QueryParam()));
		}

		[HttpGet("categories/CategoryName")]
		public IActionResult getQuestionsbyGategoryName(string categoryName)
		{
		/*	var idofCategory = _context.Categories.Where(c => c.CategoryName == categoryName).Select(id => id.CategoryId).First();

			var categories = _context.Questions
				.Join(_context.Difficulties,
				(questionTbl => questionTbl.DifficultyId),
				(difficultyTbl => difficultyTbl.DifficultyId)
				, (_question, _difficuclty) => new
				{
					Question = _question.Question1,
					Answer = _question.Answer,
					Difficulty = _difficuclty.DifficultyName,
					CategoryName = categoryName,
					CategoryNumber = _question.CategoryId,
					
				}).Where(c => c.CategoryNumber == idofCategory).ToList();


			if (categories == null)
            {
				return NotFound();
            }

			return Ok(categories); */
/*
			if (categories == null)
			{
				return NotFound();
			}
			return Ok(categories);*/

			/*var categories = _context.Categories
				.Where(name => name.CategoryName == categoryName)
				.Include(questions => questions.Questions);*/



			/*if (categories == null)
			{
				return NotFound();
			}*/

			 PaginationHandler pg = new PaginationHandler(_context.Categories, _context.Difficulties);

			 var thisWholeTHing = _context.Questions.Where(x => x.Category.CategoryName.ToLower() == categoryName.ToLower());

			 return Ok(pg.paginateQuestions(thisWholeTHing, new QueryParam()));
		}


		[HttpGet("difficulty/level")]
		public IActionResult getQuestionsByDifficulty(string level)
		{
			var difficultyLevelID = _context.Difficulties.Where(difficullyLevel => difficullyLevel.DifficultyName == level).Select(levelID => levelID.DifficultyId).First();
			
			var questions = _context.Questions.Where(difficultyTbl => difficultyTbl.DifficultyId == difficultyLevelID);

			PaginationHandler pg = new PaginationHandler(_context.Categories, _context.Difficulties);


			if (questions == null)
			{
				return NotFound();
			}

			return Ok(pg.paginateQuestions(questions, new QueryParam()));
		}

		[HttpGet("Status/statusCode")]
		public IActionResult getQuestionsByStatusCode(string statusCode)
        {
			var statusID = _context.Statuses.Where(statusName => statusName.StatusName == statusCode).Select(statusID => statusID.StatusId).First();

			var questions = _context.Questions.Where(q => q.StatusId == statusID);

			if (questions == null)
            {
				return NotFound();
            }

			PaginationHandler pg = new PaginationHandler(_context.Categories, _context.Difficulties);
			

			return Ok(pg.paginateQuestions(questions, new QueryParam()));
			
        }




		[HttpPatch("{id}")]
		public IActionResult updateStatus(int id, [FromBody] QuestionDTO questionForStatusUpdate)
		{
			Question? questionToChange = _context.Questions.Find(id);

			if (questionToChange == null)
			{
				return NotFound();
			}

			if (!string.IsNullOrEmpty(questionForStatusUpdate.Status))
			{
				Status? statusToAdd = _valueToIdUtil.getStatusByObject(questionForStatusUpdate.Status);
				if (statusToAdd == null)
				{
					string cats = String.Join(", ", _context.Statuses.Select(cat => cat.StatusName)) + ".";
					return BadRequest("Please pass in a valid status name from the list provided: " + cats);
				}
				if (statusToAdd.StatusId != questionToChange.StatusId)
				{
					questionToChange.Status = statusToAdd;
					questionToChange.StatusId = questionToChange.Status.StatusId;
				}

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
				Console.WriteLine(e.Message);
				return BadRequest("Invalid Values");
			}
		}

		[HttpPatch("mainPatch/{id}")]
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
				Difficulty? difficultyToAdd = _valueToIdUtil.getDifficultyObject(questionPatches.Difficulty);
				if (difficultyToAdd == null)
				{
					string cats = String.Join(", ", _context.Difficulties.Select(cat => cat.DifficultyName)) + ".";
					return BadRequest("Please pass in a valid difficulty name from the list provided: " + cats);
				}
				if (difficultyToAdd.DifficultyId != questionToChange.DifficultyId)
				{
					questionToChange.Difficulty = difficultyToAdd;
					questionToChange.DifficultyId = questionToChange.Difficulty.DifficultyId;
				}
			}

			if (!string.IsNullOrEmpty(questionPatches.Category))
			{
				Category? categoryToAdd = _valueToIdUtil.getCategoryObject(questionPatches.Category);
				if (categoryToAdd == null)
				{
					string cats = String.Join(", ", _context.Categories.Select(cat => cat.CategoryName)) + ".";
					return BadRequest("Please pass in a valid category name from the list provided: " + cats);
				}
				if (categoryToAdd.CategoryId != questionToChange.CategoryId)
				{
					questionToChange.Category = categoryToAdd;
					questionToChange.CategoryId = questionToChange.Category.CategoryId;
				}

			}

			if (questionPatches.Tags != null)
			{
				if (questionPatches.Tags.Length > 0)
				{
					QuestionTag tagsToAdd = new();
					tagsToAdd.QuestionId = id;
					tagsToAdd.Question = questionToChange;

					foreach (string tag in questionPatches.Tags)
					{
						Tag? tagObject = _valueToIdUtil.getTagObject(tag);
						if (tagObject == null)
						{
							return BadRequest();

						}
						if (_context.QuestionTags.Select(s => s.QuestionId == id && s.TagId == tagObject.TagId) != null)
						{
							tagsToAdd.TagId = tagObject.TagId;
                            tagsToAdd.Tag = tagObject;
						}
						
					};
				}
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

			if (string.IsNullOrEmpty(updatedQuestion.Question) ||
				string.IsNullOrEmpty(updatedQuestion.Answer) ||
				string.IsNullOrEmpty(updatedQuestion.Difficulty) ||
				string.IsNullOrEmpty(updatedQuestion.Category) ||
				string.IsNullOrEmpty(updatedQuestion.Status) ||
				updatedQuestion.Tags == null)
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

			if (updatedQuestion.Tags.Length > 0)
			{
				QuestionTag tagsToAdd = new();
				tagsToAdd.QuestionId = id;
				tagsToAdd.Question = question;

				foreach (string tag in updatedQuestion.Tags)
				{
					Tag? tagObject = _valueToIdUtil.getTagObject(tag);
					if (tagObject == null)
					{
						return BadRequest(_valueToIdUtil.getInvalidTagResponse());
					}

					if (_context.QuestionTags.Select(s => s.QuestionId == id && s.TagId == tagObject.TagId) != null)
					{
						tagsToAdd.TagId = tagObject.TagId;
						tagsToAdd.Tag = tagObject;
					}
				};
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
				return BadRequest("Question probably exists or an error occurred in our side");
			}
		}


        [HttpPost]
        public IActionResult insertQuestion([FromBody] QuestionDTO newQuestionDetails)
        {


			if (string.IsNullOrEmpty(newQuestionDetails.Question) ||
				string.IsNullOrEmpty(newQuestionDetails.Answer) ||
				string.IsNullOrEmpty(newQuestionDetails.Difficulty) ||
				string.IsNullOrEmpty(newQuestionDetails.Category) ||
				string.IsNullOrEmpty(newQuestionDetails.Status) ||
				newQuestionDetails.Tags == null
				)
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


			var status = _valueToIdUtil.getStatusByObject(newQuestionDetails.Status);
			if (status == null)
			{
				return BadRequest("Status does not exist");
			}
			newQuestion.Status = status;
			newQuestion.StatusId = status.StatusId;



			Console.WriteLine(JsonSerializer.Serialize(newQuestion));
			try
			{
				_context.Questions.Update(newQuestion);
				_context.SaveChanges();
				
				
                if(newQuestionDetails.Tags != null)
                {
					if(newQuestionDetails.Tags.Length > 0)
                    {
						foreach (string tag in newQuestionDetails.Tags)
						{
							QuestionTag tagsToAdd = new();
							tagsToAdd.QuestionId = newQuestion.QuestionId;
							tagsToAdd.Question = newQuestion;

							Tag tagObject = _valueToIdUtil.getTagObject(tag);
							if (tagObject == null)
							{
								return BadRequest();

							}
							tagsToAdd.TagId = tagObject.TagId;
							tagsToAdd.Tag = tagObject;

							_context.QuestionTags.Update(tagsToAdd);

						};
					}
                }

               

                _context.SaveChanges();
                return Ok(QuestionDTO.AsDTO(newQuestion, newQuestionDetails.Tags )); 
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
