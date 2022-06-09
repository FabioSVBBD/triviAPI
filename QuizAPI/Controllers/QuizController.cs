using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Model;

namespace QuizAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        TriviapiDBContext _context = new TriviapiDBContext();

        [HttpGet(Name = "all")]
        public IActionResult allQuestions([FromQuery] FilterQuery query)
        {
            List<Question> questions = _context.Questions.Include("Difficulty").ToList();
            
            return new ObjectResult(query.filterAll(questions));
        }
    }
}
