using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAPI.Model;

namespace QuizAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        TriviapiDBContext _context = new TriviapiDBContext();

        [HttpGet(Name = "test")]
        public IActionResult testMe()
        {
            return new ObjectResult(_context.Questions.Find(1));
        }
    }
}
