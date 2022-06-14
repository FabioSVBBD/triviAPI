using QuizAPI.Model;
using System.Linq;
using QuizAPI.DTOs;

    public sealed class InvalidResponseUtil
    {

	private InvalidResponseUtil() { }

	private static InvalidResponseUtil _invalidResponseUtil;

	public static InvalidResponseUtil getInstance()
    {
		if(_invalidResponseUtil == null)
        {
			_invalidResponseUtil = new InvalidResponseUtil();
		}
		return _invalidResponseUtil;
    }

	TriviapiDBContext _context = new TriviapiDBContext();
		public InvalidResponseDTO getInvalidDifficultyResponse()
		{
			return new InvalidResponseDTO("Invalid Difficulty", _context.Difficulties.ToList().Select(d => d.DifficultyName).ToList());
		}

		public InvalidResponseDTO getInvalidCategoryResponse()
		{
			return new InvalidResponseDTO("Invalid Category", _context.Categories.ToList().Select(d => d.CategoryName).ToList());
		}

		public InvalidResponseDTO getInvalidStatusResponse()
		{
			return new InvalidResponseDTO("Invalid Status", _context.Statuses.ToList().Select(s => s.StatusName).ToList());
		}

		public InvalidResponseDTO getInvalidTagResponse()
		{
			return new InvalidResponseDTO("Invalid Tag/s", _context.Tags.ToList().Select(t => t.TagName).ToList());
		}

		public Dictionary<String, String> getPendingFailResponse()
		{
			return new() { { "message", "An issue occurred setting status to pending. We have some maintenance to do." } };
		}

		public Dictionary<String, String> getDeletedFailResponse()
		{
			return new() { { "message", "An issue occurred setting setting status to deleted. We have some maintenance to do." } };
		}
	}
