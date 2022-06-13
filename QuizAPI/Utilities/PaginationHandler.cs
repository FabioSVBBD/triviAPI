using QuizAPI.Model;
using QuizAPI.DTOs;
using QuizAPI.Utils;
namespace QuizAPI.Utilities;
public class PaginationHandler
{
    public int count { get; set; } = 0;
    public int page { get; set; } = 1;
    public int pageSize { get; set; } = 20;
    public string next { get; set; } = String.Empty;
    public string back { get; set; } = String.Empty;
    public List<QuestionData> results { get; set; } = new List<QuestionData>();

    private IQueryable<Category> categoryQS;
    private IQueryable<Difficulty> difficultyQS;
    private TriviapiDBContext _context = new TriviapiDBContext();

    public class QuestionData
    {
        public string question { get; set; }
        public string answer { get; set; }
        public string difficulty { get; set; }
        public string category { get; set; }
        public List<string> tags { get; set; }

        public QuestionData(String question, String answer, String category, String difficulty, List<string> tags)
        {
            this.question = question;
            this.answer = answer;
            this.difficulty = difficulty;
            this.category = category;
            this.tags = tags;
        }
    }

    public PaginationHandler(IQueryable<Category> categoryQS, IQueryable<Difficulty> difficultyQS)
    {
        this.categoryQS = categoryQS;
        this.difficultyQS = difficultyQS;
    }

    public PaginationHandler paginateQuestions(IQueryable<Question> questionQS, QueryParam query)
    {
        IQueryable<Question> approvedQuerySet = questionQS.Where(ques => ques.Status.StatusName.ToLower() == "approved");

        Question question = approvedQuerySet.First();

        approvedQuerySet.Skip((query.Page - 1) * this.pageSize).Take(pageSize).ToList().ForEach(
            x => results.Add(
                new QuestionData(
                    x.Question1, x.Answer, x.Category.CategoryName, x.Difficulty.DifficultyName, generateTags(x.QuestionId)
             )
          )
        );

        page = query.Page;
        count = approvedQuerySet.Count();
        next = count - pageSize * page > 0 ? buildURL(query, true) : String.Empty;
        back = page > 1 ? buildURL(query, false) : String.Empty;  

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

    private string buildURL(QueryParam query, bool next)
    {
        string ret = String.Empty;
        int pageModifier = next ? 1 : -1;
        List<string> paramList = new List<string>();

        paramList.Add(string.Format("Page={0}", query.Page + pageModifier));
        
        query.Categories.ForEach(
            cat => paramList.Add(
               string.Format("Categories={0}", cat) 
           )
        );

        query.Difficulties.ForEach(
            diff => paramList.Add(
                string.Format("Difficulties={0}", diff)
           )
        );

        string parameters = paramList.Count > 0 ? "?" + String.Join("&", paramList) : String.Empty;

        return $"{UrlHelper.baseURL}{parameters}";
    }
}