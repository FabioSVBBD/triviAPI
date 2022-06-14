using QuizAPI.Model;
using QuizAPI.Validation;
using Microsoft.AspNetCore.Mvc;
namespace QuizAPI.Utils;
public class FilterQuery
{
    public QueryValidator queryValidator; public string CurrentUrl { get; set; } = String.Empty; private IQueryable<Question> foundData = new List<Question>().AsQueryable(); private IQueryable<Question> approvedQuestions;
    private TriviapiDBContext _context = new TriviapiDBContext(); public FilterQuery(QueryValidator validData, HttpRequest request)
    {
        string scheme = request.Scheme;
        string host = request.Host.Value;
        string path = request.Path; CurrentUrl = $"{scheme}://{host}{path}"; queryValidator = validData;
        approvedQuestions = _context.Questions.Where(x => x.Status.StatusName == "approved");
    }
    private void byCategory()
    {
        if (foundData.Count() != 0)
        {
            bool countedCategories = queryValidator.categories.Count > 0; foundData = !countedCategories ? foundData : foundData.Where(x =>
            queryValidator.categories.Contains(x.Category.CategoryName.ToLower())
            );
        }
        else
        {
            List<Question> categoryQuestions = approvedQuestions.Where(
            x => queryValidator.categories.Contains(x.Category.CategoryName.ToLower())
            ).ToList(); categoryQuestions.ForEach(x => addToFoundData(x));
        }
    }
    private void addToFoundData(Question question)
    {
        foundData.ToList().ForEach(x =>
        {
            if (x.Question1 == question.Question1)
                return;
        }
        ); foundData = foundData.Append(question);
    }
    private void byDifficulty()
    {
        if (foundData.Count() != 0)
        {
            bool countedDifficulties = queryValidator.difficulties.Count > 0;
            Console.WriteLine(queryValidator.difficulties.Count); foundData = !countedDifficulties ? foundData : foundData.Where(x =>
            queryValidator.difficulties.Contains(x.Difficulty.DifficultyName.ToLower())
            );
        }
        else
        {
            List<Question> difficultyQuestions = foundData.Where(
            x => queryValidator.difficulties.Contains(x.Difficulty.DifficultyName.ToLower())
            ).ToList(); difficultyQuestions.ForEach(x => addToFoundData(x));
        }
    }
    private void byTag()
    {
        IQueryable<QuestionTag> qt = _context.QuestionTags.Where(x => queryValidator.tags.Contains(x.Tag.TagName.ToLower()));
        if (foundData.Count() != 0)
        {
            bool countedTags = queryValidator.tags.Count > 0;
            foundData = !countedTags ? foundData : foundData.Where(x =>
            qt.Any(tag => tag.QuestionId == x.QuestionId)
            );
        }
        else
        {
            List<Question> tagQuestions = approvedQuestions.Where(
            x => qt.Any(tag => tag.QuestionId == x.QuestionId)
            ).ToList();
            tagQuestions.ForEach(x => addToFoundData(x));
        }
    }
    public ObjectResult byAll()
    {
        this.byTag();
        this.byCategory();
        this.byDifficulty(); if (foundData.Count() == 0)
            return new ObjectResult("Nothing");
        return new ObjectResult(new PaginationHandler().paginateQuestions(foundData, this));
    }
}

