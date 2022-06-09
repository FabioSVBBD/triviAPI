using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAPI.Model;
using QuizAPI.Utils;

namespace QuizAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        TriviapiDBContext _context = new TriviapiDBContext();
        ValueToIdUtil _valueToIdUtil = new ValueToIdUtil();

        [HttpGet(Name = "test")]
        public IActionResult testMe()
        {
            return new ObjectResult(_context.Questions.Find(1));
        }


        [HttpPatch]
        public Question updateQuestion(int id, [FromBody] Question questionPatches)
        {
            Question questionToChange = _context.Questions.Find(id);

            if (!string.IsNullOrEmpty(questionPatches.Question1))
            {
                questionToChange.Question1 = questionPatches.Question1;
            }

            if (!string.IsNullOrEmpty(questionPatches.Answer))
            {
                questionToChange.Answer = questionPatches.Answer;
            }

            if (!string.IsNullOrEmpty(questionPatches.Difficulty.DifficultyName))
            {
                int difficultyForPatchId = _valueToIdUtil.getDifficulty(questionPatches.Difficulty.DifficultyName);
                questionToChange.DifficultyId = questionToChange.Difficulty.DifficultyId = difficultyForPatchId;
                questionToChange.Difficulty.DifficultyName = questionPatches.Difficulty.DifficultyName;
                _context.Questions.Update(questionToChange);
                _context.SaveChanges();
            }

            if (!string.IsNullOrEmpty(questionPatches.Category.CategoryName))
            {
                int categoryId = _valueToIdUtil.getCategory(questionPatches.Category.CategoryName);
                questionToChange.CategoryId = questionToChange.Category.CategoryId = categoryId;
                questionToChange.Category.CategoryName = questionPatches.Category.CategoryName;
            }

            if (!string.IsNullOrWhiteSpace(questionPatches.Status.StatusName))
            {
                int statusId = _valueToIdUtil.getStatus(questionPatches.Status.StatusName);
                questionToChange.StatusId = questionToChange.Status.StatusId = statusId;
                questionToChange.Status.StatusName = questionPatches.Status.StatusName;
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
