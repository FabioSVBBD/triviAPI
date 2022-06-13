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

    public class QuestionData
    {
        public string question { get; set; }
        public string answer { get; set; }
        public string difficulty { get; set; }
        public string category { get; set; }

        public QuestionData(String question, String answer, String category, String difficulty)
        {
            this.question = question;
            this.answer = answer;
            this.difficulty = difficulty;
            this.category = category;
        }
    }

    public PaginationHandler(IQueryable<Category> categoryQS, IQueryable<Difficulty> difficultyQS)
    {
        this.categoryQS = categoryQS;
        this.difficultyQS = difficultyQS;
    }

    public PaginationHandler paginateQuestions(IQueryable<Question> questionQS, QueryParam query)
    {
        /*lts = questionQS.Skip((query.Page - 1) * this.pageSize).Take(pageSize).Join(
            categoryQS,
            (q => q.CategoryId),
            (c => c.CategoryId),
            (_question, _category) =>
                new QuestionData(_question.Question1, _question.Answer, _category.CategoryName,
                difficultyQS.First(x => x.DifficultyId == _question.DifficultyId).DifficultyName
            )
         ).ToList();*/

        questionQS.Skip((query.Page - 1) * this.pageSize).Take(pageSize).ToList().ForEach(
            x => results.Add(
                new QuestionData(x.Question1, x.Answer, x.Category.CategoryName, x.Difficulty.DifficultyName)
          )
        );

        page = query.Page;
        count = questionQS.Count();
        next = count - pageSize * page > 0 ? buildURL(query, true) : String.Empty;
        back = page > 1 ? buildURL(query, false) : String.Empty;  

        return this;
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