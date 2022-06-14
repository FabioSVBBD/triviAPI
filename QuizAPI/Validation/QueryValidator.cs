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
    ForeignKeyObjectsUtil _foreignKeyObjectsUtil = new ForeignKeyObjectsUtil();

    public QueryValidator(
        QueryParam queryParam
     )
    {
        this.page = queryParam.Page;
        this.difficulties = validateDifficulties(queryParam.Difficulties);
        this.tags = validateTags(queryParam.Tags) ? queryParam.Tags : new();
        this.categories = validateCategories(queryParam.Categories);
    }

    public bool isValid()
    {
        Console.WriteLine(errors.Count);
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

    public bool validateTags(List<string> tags)
    {
        foreach (string tag in tags)
        {
            Tag? tagObject = _foreignKeyObjectsUtil.getTagObject(tag);
            if (tagObject == null)
            {
                Console.WriteLine("in block");
                errors.Add(new InvalidResponseUtil().getInvalidCategoryResponse());
                return false;
            }
        }
        return true;
    }
}