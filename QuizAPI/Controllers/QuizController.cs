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
                    difficultyToAdd.Questions.Add(questionToChange);
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
                    categoryToAdd.Questions.Add(questionToChange);
                }
            }

            if (questionPatches.Tags != null)
            {
                if (questionPatches.Tags.Length > 0)
                {
                    foreach (string tag in questionPatches.Tags)
                    {
                        Tag? tagObject = _valueToIdUtil.getTagObject(tag);
                        QuestionTag tagsToAdd = new();
                        if (tagObject == null)
                        {
                            return BadRequest();
                        }
                        if (_context.QuestionTags.Select(s => s.QuestionId == id && s.TagId == tagObject.TagId) != null)
                        {
                            tagsToAdd.QuestionId = id;
                            tagsToAdd.Question = questionToChange;
                            tagsToAdd.TagId = tagObject.TagId;
                            tagsToAdd.Tag = tagObject;
                            tagObject.QuestionTags.Add(tagsToAdd);
                            questionToChange.QuestionTags.Add(tagsToAdd);
                            _context.QuestionTags.Update(tagsToAdd);
                        }
                    };
                }
            }

            Status? pending = _valueToIdUtil.getStatusByObject("pending");

						if (pending == null) {
							return StatusCode(500, _valueToIdUtil.getPendingFailResponse());
						}

            questionToChange.Status = pending;
            questionToChange.StatusId = pending.StatusId;
            pending.Questions.Add(questionToChange);

            try
            {
                _context.Questions.Update(questionToChange);
                _context.SaveChanges();

                List<string> tagsToReturn = (from t in _context.Tags
                                             join qt in _context.QuestionTags
                                             on t.TagId equals qt.TagId
                                             where qt.QuestionId == questionToChange.QuestionId
                                             select new
                                             {
                                                 tagName = t.TagName
                                             }).ToList().Select(t => t.tagName).ToList();

                return Ok(QuestionDTO.AsDTO(_context.Questions.Find(questionToChange.QuestionId), tagsToReturn.ToArray()));
            }
            catch (Exception e)
            {
                _ = e;
                Console.WriteLine(e);
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
                updatedQuestion.Tags == null)
            {
                return BadRequest("You are missing some fields");
            }

            question.Question1 = updatedQuestion.Question;
            question.Answer = updatedQuestion.Answer;

            Difficulty? fetchedDifficulty = _valueToIdUtil.getDifficultyObject(updatedQuestion.Difficulty);

            if (fetchedDifficulty == null)
            {
                return BadRequest(_valueToIdUtil.getInvalidDifficultyResponse());
            }
            if (question.Difficulty.DifficultyId != fetchedDifficulty.DifficultyId)
            {
                question.Difficulty = fetchedDifficulty;
                question.DifficultyId = question.Difficulty.DifficultyId;
                fetchedDifficulty.Questions.Add(question);
            }
            Category? fetchedCategory = _valueToIdUtil.getCategoryObject(updatedQuestion.Category);

            if (fetchedCategory == null)
            {
                return BadRequest(_valueToIdUtil.getInvalidCategoryResponse());
            }
            if (question.Category.CategoryId != fetchedCategory.CategoryId)
            {
                question.Category = fetchedCategory;
                question.CategoryId = question.Category.CategoryId;
                fetchedCategory.Questions.Add(question);
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
                        tagsToAdd.QuestionId = id;
                        tagsToAdd.Question = question;
                        tagsToAdd.TagId = tagObject.TagId;
                        tagsToAdd.Tag = tagObject;
                        tagObject.QuestionTags.Add(tagsToAdd);
                        question.QuestionTags.Add(tagsToAdd);
                        _context.QuestionTags.Update(tagsToAdd);
                    }
                };
            }

            Status? pending = _valueToIdUtil.getStatusByObject("pending");

						if (pending == null) {
							return StatusCode(500, _valueToIdUtil.getPendingFailResponse());
						}
						
            question.Status = pending;
            question.StatusId = pending.StatusId;
            pending.Questions.Add(question);

            try
            {
                _context.Questions.Update(question);
                _context.SaveChanges();
                List<string> tagsToReturn = (from t in _context.Tags
                                             join qt in _context.QuestionTags
                                             on t.TagId equals qt.TagId
                                             where qt.QuestionId == question.QuestionId
                                             select new
                                             {
                                                 tagName = t.TagName
                                             }).ToList().Select(t => t.tagName).ToList();

                return Ok(QuestionDTO.AsDTO(_context.Questions.Find(question.QuestionId), tagsToReturn.ToArray()));
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

            try
            {
                _context.Questions.Update(newQuestion);
                _context.SaveChanges();

                if (newQuestionDetails.Tags != null)
                {
                    if (newQuestionDetails.Tags.Length > 0)
                    {
                        foreach (string tag in newQuestionDetails.Tags)
                        {
                            QuestionTag tagsToAdd = new();
                            tagsToAdd.QuestionId = newQuestion.QuestionId;
                            tagsToAdd.Question = newQuestion;

                            Tag? tagObject = _valueToIdUtil.getTagObject(tag);
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
                return Ok(QuestionDTO.AsDTO(newQuestion, newQuestionDetails.Tags));
            }
            catch (Exception e)
            {
                _ = e;
                Console.WriteLine(e);
                return BadRequest("Failed To Connect to Database"); ;
            }
        }

		[HttpDelete("{id}")]
		public IActionResult deleteQuestion(int id)
		{
			Question? question = _context.Questions.Find(id);

			if (question == null)
			{
				return NotFound();
			}

			var deletedStatus = _valueToIdUtil.getStatusByObject("deleted");

			if (deletedStatus == null) {
				return StatusCode(500, _valueToIdUtil.getDeletedFailResponse());
			}

			question.Status = deletedStatus;

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
	}
}
