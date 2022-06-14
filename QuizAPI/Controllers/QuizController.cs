using Microsoft.AspNetCore.Mvc;
using QuizAPI.Model;
using QuizAPI.Utils;
using QuizAPI.DTOs;
using System.Text.Json;
using QuizAPI.Validation;

namespace QuizAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        TriviapiDBContext _context = new TriviapiDBContext();
        ForeignKeyObjectsUtil _foreignKeyObjectsUtil = new ForeignKeyObjectsUtil();
        InvalidResponseUtil _invalidResponseUtil = new InvalidResponseUtil();

        [HttpGet("question/{id}")]

        public IActionResult getQuestionById(int id)
        {
            Question? question = _context.Questions.Where(question => question.QuestionId == id).ToList().FirstOrDefault();

            if (question == null || !question.Status.StatusName.Equals("approved"))
            {
                return NotFound();
            }
            else
            {
                return Ok(QuestionDTO.AsDTO(question, _foreignKeyObjectsUtil.getTagsForQuestion(id).ToArray()));
            }
        }

        [HttpGet("questions")]
        public IActionResult getAllQuestions([FromQuery] QueryParam parameters)

        {

            QueryValidator queryValidator = new QueryValidator(parameters);

            if (queryValidator.isValid())

            {

                var currentData = queryValidator.data(Request);

                return Ok(currentData.byAll());

            }



            return BadRequest(queryValidator.validationErrors());

        }


        [HttpGet("difficulties")]
        public IActionResult getAllDifficulties()
        {
            var difficulties = _context.Difficulties.Select(diff => diff.DifficultyName);

            if (difficulties.Count() == 0)
            {
                return NotFound();
            }

            return Ok(difficulties);
        }

        [HttpGet("categories")]
        public IActionResult getallCategories()
        {
            var categories = _context.Categories.Select(c => c.CategoryName);

            if (categories.Count() == 0)
            {
                return NotFound();
            }

            return Ok(categories);
        }

        [HttpGet("statuses")]
        public IActionResult getAllStatuses()
        {
            var statuses = _context.Statuses.Select(status => status.StatusName);

            if (statuses.Count() == 0)
            {
                return NotFound();
            }
            return Ok(statuses);
        }

        [HttpGet("tags")]
        public IActionResult getAllTags()
        {
            var tags = _context.Tags.Select(tag => tag.TagName);

            if (tags.Count() == 0)
            {
                return NotFound();
            }
            return Ok(tags);
        }




        [HttpPatch("question/status/{id}")]
        public IActionResult updateStatus(int id, [FromBody] StatusDTO status)
        {
            Question? questionToChange = _context.Questions.Find(id);
            if (questionToChange == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(status.Status))
            {

                Status? statusToAdd = _foreignKeyObjectsUtil.getStatus(status.Status);

                if (statusToAdd == null)
                {
                    return BadRequest(_invalidResponseUtil.getInvalidStatusResponse());
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

                return Ok();
            }
            catch (Exception e)
            {
                _ = e;
                return BadRequest(_invalidResponseUtil.getInvalidValuesResponse());
            }
        }

        [HttpPatch("question/{id}")]
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
                Difficulty? difficultyToAdd = _foreignKeyObjectsUtil.getDifficulty(questionPatches.Difficulty);
                if (difficultyToAdd == null)
                {
                    return BadRequest(_invalidResponseUtil.getInvalidDifficultyResponse());
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
                Category? categoryToAdd = _foreignKeyObjectsUtil.getCategory(questionPatches.Category);
                if (categoryToAdd == null)
                {
                    return BadRequest(_invalidResponseUtil.getInvalidCategoryResponse());
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
                        Tag? tagObject = _foreignKeyObjectsUtil.getTag(tag);
                        QuestionTag tagsToAdd = new();
                        if (tagObject == null)
                        {
                            return BadRequest(_invalidResponseUtil.getInvalidTagResponse());
                        }
                        if (_context.QuestionTags.Where(s => s.QuestionId == id && s.TagId == tagObject.TagId).ToList().Count == 0)
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

            Status? pending = _foreignKeyObjectsUtil.getStatus("pending");

            if (pending == null)
            {
                return StatusCode(500, _invalidResponseUtil.getPendingFailResponse());
            }
            else
            {
                if (!(pending.StatusId == questionToChange.Status.StatusId))
                {
                    questionToChange.Status = pending;
                    questionToChange.StatusId = pending.StatusId;
                    pending.Questions.Add(questionToChange);
                }
            }

            try
            {
                _context.Questions.Update(questionToChange);
                _context.SaveChanges();

                List<string> tagsToReturn = _foreignKeyObjectsUtil.getTagsForQuestion(questionToChange.QuestionId);

                return Ok(QuestionDTO.AsDTO(_context.Questions.Find(questionToChange.QuestionId), tagsToReturn.ToArray()));
            }
            catch (Exception e)
            {
                _ = e;
                Console.WriteLine(e);
                return BadRequest(_invalidResponseUtil.getInvalidValuesResponse());
            }
        }

        [HttpPut("question/{id}")]
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
                return BadRequest(_invalidResponseUtil.getMissingFieldsResponse());
            }

            question.Question1 = updatedQuestion.Question;
            question.Answer = updatedQuestion.Answer;

            Difficulty? fetchedDifficulty = _foreignKeyObjectsUtil.getDifficulty(updatedQuestion.Difficulty);

            if (fetchedDifficulty == null)
            {
                return BadRequest(_invalidResponseUtil.getInvalidDifficultyResponse());
            }
            if (question.Difficulty.DifficultyId != fetchedDifficulty.DifficultyId)
            {
                question.Difficulty = fetchedDifficulty;
                question.DifficultyId = question.Difficulty.DifficultyId;
                fetchedDifficulty.Questions.Add(question);
            }
            Category? fetchedCategory = _foreignKeyObjectsUtil.getCategory(updatedQuestion.Category);

            if (fetchedCategory == null)
            {
                return BadRequest(_invalidResponseUtil.getInvalidCategoryResponse());
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
                    Tag? tagObject = _foreignKeyObjectsUtil.getTag(tag);
                    if (tagObject == null)
                    {
                        return BadRequest(_invalidResponseUtil.getInvalidTagResponse());
                    }

                    if (_context.QuestionTags.Where(s => s.QuestionId == id && s.TagId == tagObject.TagId).ToList().Count == 0)
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

            Status? pending = _foreignKeyObjectsUtil.getStatus("pending");

            if (pending == null)
            {
                return StatusCode(500, _invalidResponseUtil.getPendingFailResponse());
            }
            else
            {
                if (!(pending.StatusId == question.Status.StatusId))
                {
                    question.Status = pending;
                    question.StatusId = pending.StatusId;
                    pending.Questions.Add(question);
                }
            }

            try
            {
                _context.Questions.Update(question);
                _context.SaveChanges();
                List<string> tagsToReturn = _foreignKeyObjectsUtil.getTagsForQuestion(question.QuestionId);

                return Ok(QuestionDTO.AsDTO(_context.Questions.Find(question.QuestionId), tagsToReturn.ToArray()));
            }
            catch (Exception e)
            {
                _ = e;
                return StatusCode(500, _invalidResponseUtil.getGenericErrorResponse());
            }
        }


        [HttpPost("question")]
        public IActionResult insertQuestion([FromBody] QuestionDTO newQuestionDetails)
        {
            if (string.IsNullOrEmpty(newQuestionDetails.Question) ||
                string.IsNullOrEmpty(newQuestionDetails.Answer) ||
                string.IsNullOrEmpty(newQuestionDetails.Difficulty) ||
                string.IsNullOrEmpty(newQuestionDetails.Category) ||
                newQuestionDetails.Tags == null
                )
            {
                return BadRequest(_invalidResponseUtil.getMissingFieldsResponse());
            }

            var newQuestion = new Question();
            newQuestion.Question1 = newQuestionDetails.Question;
            newQuestion.Answer = newQuestionDetails.Answer;

            var category = _foreignKeyObjectsUtil.getCategory(newQuestionDetails.Category);
            if (category == null)
            {
                return BadRequest(_invalidResponseUtil.getInvalidCategoryResponse());
            }

            newQuestion.Category = category;
            newQuestion.CategoryId = category.CategoryId;

            var difficulty = _foreignKeyObjectsUtil.getDifficulty(newQuestionDetails.Difficulty);
            if (difficulty == null)
            {
                return BadRequest(_invalidResponseUtil.getInvalidDifficultyResponse());
            }
            newQuestion.Difficulty = difficulty;
            newQuestion.DifficultyId = difficulty.DifficultyId;

            var status = _foreignKeyObjectsUtil.getStatus("pending");

            if (status == null)
            {
                return BadRequest(_invalidResponseUtil.getInvalidStatusResponse());
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

                            Tag? tagObject = _foreignKeyObjectsUtil.getTag(tag);
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
                return StatusCode(500, _invalidResponseUtil.getGenericErrorResponse());
            }
        }

        [HttpDelete("question/{id}")]
        public IActionResult deleteQuestion(int id)
        {
            Question? question = _context.Questions.Find(id);

            if (question == null)
            {
                return NotFound();
            }

            var deletedStatus = _foreignKeyObjectsUtil.getStatus("deleted");

            if (deletedStatus == null)
            {
                return StatusCode(500, _invalidResponseUtil.getDeletedFailResponse());
            }

            question.Status = deletedStatus;

            try
            {
                _context.Questions.Update(question);
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                _ = e;
                return BadRequest(_invalidResponseUtil.getDeleteFailedResponse());
            }
        }
    }
}
