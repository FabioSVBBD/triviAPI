using QuizAPI.Model;
using QuizAPI.DTOs;
using QuizAPI.Utils;
using Microsoft.AspNetCore.Mvc;

namespace QuizAPI.Validation;

public class QueryValidator
{
    public List<string> tags { get; set; } = new();
    public List<string> categories = new();
    public List<string> difficulties = new();

    public int page { get; set; } = 1;
    private List<InvalidResponseDTO> errors = new();
    private TriviapiDBContext _context = new TriviapiDBContext();

    public QueryValidator(
        QueryParam queryParam
     )
    {
        this.page = queryParam.Page;
        this.difficulties = validateDifficulties(queryParam.Difficulties);
        this.tags = validateTags(queryParam.Tags);
        this.categories = validateCategories(queryParam.Categories);
    }

    public bool isValid()
    {
        return errors.Count == 0;
    }

    public List<InvalidResponseDTO> validationErrors()
    {
        return errors;
    }

    public FilterQuery data(HttpRequest request)
    {
        return new FilterQuery(this, request);
    }

    public List<string> validateDifficulties(List<string> userList)
    {
        userList.ForEach(x => x.ToLower());

        List<string> result = new();
        IQueryable<Difficulty> difficultyQS = _context.Difficulties;

        foreach (string difficulty in userList)
        {
            bool keep = difficultyQS.Any(x => x.DifficultyName.ToLower() == difficulty);

            if (keep)
               result.Add(difficulty);
        }

        if (difficulties.Count > 0 && result.Count == 0)
            errors.Add(new InvalidResponseUtil().getInvalidDifficultyResponse());

        Console.WriteLine($"so uhm {result.Count}");
        return result;
    }

    public List<string> validateCategories(List<string> categories)
    {
        categories.ForEach(x => x.ToLower());

        List<string> result = new();
        IQueryable<Category> categoryQS = _context.Categories;

        foreach (string category in categories)
        {
            bool keep = categoryQS.Any(x => x.CategoryName.ToLower() == category);

            if (keep)
              result.Add(category);
        }

        if (categories.Count > 0 && result.Count == 0)
            errors.Add(new InvalidResponseUtil().getInvalidCategoryResponse());

        return result;
    }

    public List<string> validateTags(List<string> tags)
    {
        tags.ForEach(x => x.ToLower());

        List<string> result = new();
        IQueryable<Tag> tagQS = _context.Tags;

        foreach (string tag in tags)
        {
            bool keep = tagQS.Any(x => x.TagName.ToLower() == tag);

            if (keep)
                result.Add(tag);
        }

        if (categories.Count > 0 && result.Count == 0)
            errors.Add(new InvalidResponseUtil().getInvalidTagResponse());

        return result;
    }
}