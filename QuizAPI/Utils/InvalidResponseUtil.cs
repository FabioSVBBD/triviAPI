﻿using QuizAPI.Model;
using System.Linq;
using QuizAPI.DTOs;

    public class InvalidResponseUtil
    {

	TriviapiDBContext _context = new TriviapiDBContext();

	static String message = "message";
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
			return new() { { message, "An issue occurred setting status to pending. We have some maintenance to do." } };
		}

		public Dictionary<String, String> getDeletedFailResponse()
		{
			return new() { { message, "An issue occurred setting setting status to deleted. We have some maintenance to do." } };
		}

		public Dictionary<String, String> getGenericErrorResponse()
		{
			return new() { { message, "An error occurred on our side" } };
		}

		public Dictionary<String, String> getInvalidValuesResponse()
		{
			return new() { { message, "Invalid Values" } };
		}

		public Dictionary<String, String> getMissingFieldsResponse()
		{
			return new() { { message, "You are missing some fields" } };
		}

		public Dictionary<String, String> getDeleteFailedResponse()
		{
			return new() { { message, "Question does not exist" } };
		}
	}
