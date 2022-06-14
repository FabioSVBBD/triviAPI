using QuizAPI.Model;
using QuizAPI.DTOs;
using QuizAPI.Utils;
namespace QuizAPI.Utilities;
public class PaginationHandler
{
    public int count { get; set; } = 0;
    public int page { get; set; } = 1;
    public int pageSize { get; set; } = 20;
    public string next { get; set; } = string.Empty;
    public string back { get; set; } = string.Empty;
    public List<QuestionDTO> results { get; set; } = new List<QuestionDTO>();

    private TriviapiDBContext _context = new TriviapiDBContext();

    public PaginationHandler paginateQuestions(IQueryable<Question> questionQS, FilterQuery query)
    {
        questionQS.Skip((query.queryValidator.page - 1) * pageSize).Take(pageSize).ToList().ForEach(
            x => results.Add(
                new QuestionDTO(
                    x.Question1, x.Answer, x.Difficulty.DifficultyName, x.Category.CategoryName, generateTags(x.QuestionId).ToArray()
             )
          )
        );

        page = query.queryValidator.page;
        count = questionQS.Count();
        next = count - pageSize * page > 0 ? buildURL(query, true) : string.Empty;
        back = page > 1 ? buildURL(query, false) : string.Empty;

        return this;
    }

    private List<string> generateTags(int questionID)
    {
        List<string> ret = new();

        var tagsQuerySet = _context.Tags;
        var filteredQuestionTags = _context.QuestionTags.Where(x => x.QuestionId == questionID).ToList();
        filteredQuestionTags.ForEach(x => {
            Tag? foundTag = tagsQuerySet.Find(x.TagId);

            if (foundTag != null)
            {
                ret.Add(foundTag.TagName);
            }
          }
        );
        return ret;
    }

    private string buildURL(FilterQuery query, bool next)
    {
        string ret = string.Empty;
        int pageModifier = next ? 1 : -1;
        List<string> paramList = new List<string>();

        paramList.Add(string.Format("Page={0}", query.queryValidator.page + pageModifier));

        query.queryValidator.categories.ForEach(
            cat => paramList.Add(
               string.Format("Categories={0}", cat)
           )
        );

        query.queryValidator.difficulties.ForEach(
            diff => paramList.Add(
                string.Format("Difficulties={0}", diff)
           )
        );

        string parameters = paramList.Count > 0 ? "?" + string.Join("&", paramList) : string.Empty;

        return $"{query.CurrentUrl}{parameters}";
    }
}