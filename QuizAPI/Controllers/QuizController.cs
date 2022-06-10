using Microsoft.AspNetCore.Http;
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

        [HttpGet(Name = "test")]
        public IActionResult testMe()
        {
            return new ObjectResult(_context.Questions.Find(2));
        }


        [HttpPatch]
        public Question updateQuestion(int id, [FromBody] QuestionDTO questionPatches)
        {
            Question questionToChange = _context.Questions.Find(id);

            if (!string.IsNullOrWhiteSpace(questionPatches.Question))
            {
                questionToChange.Question1 = questionPatches.Question;
            }

            if (!string.IsNullOrWhiteSpace(questionPatches.Answer))
            {
                questionToChange.Answer = questionPatches.Answer;
            }

            if (!string.IsNullOrWhiteSpace(questionPatches.Difficulty))
            {
                int difficultyForPatchId = _valueToIdUtil.getDifficulty(questionPatches.Difficulty);
                questionToChange.DifficultyId = questionToChange.Difficulty.DifficultyId = difficultyForPatchId;
                questionToChange.Difficulty.DifficultyName = questionPatches.Difficulty;
                _context.Questions.Update(questionToChange);
                _context.SaveChanges();
            }

            if (!string.IsNullOrWhiteSpace(questionPatches.Category))
            {
                int categoryId = _valueToIdUtil.getCategory(questionPatches.Category);
                questionToChange.CategoryId = questionToChange.Category.CategoryId = categoryId;
                questionToChange.Category.CategoryName = questionPatches.Category;
            }

            if (!string.IsNullOrWhiteSpace(questionPatches.Status))
            {
                int statusId = _valueToIdUtil.getStatus(questionPatches.Status);
                questionToChange.StatusId = questionToChange.Status.StatusId = statusId;
                questionToChange.Status.StatusName = questionPatches.Status;
            }

            try
            {
                _context.Questions.Update(questionToChange);
                _context.SaveChanges();
                return _context.Questions.Find(id);
            }
            catch (Exception e)
            {
                throw new Exception("Invalid values");
            }
            return null;


        }
        }
}
