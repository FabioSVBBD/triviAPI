namespace QuizAPI.Model;

public class FilterQuery
{
    public int page { get; set; } = 1;
    private List<string> Categories = new List<string>();
    private List<string> Difficulties = new List<string>();
    public int limit { get; set; } = 0;

    public List<string> categories
    {
        get { return Categories; }
        set
        {
            value.ForEach(x => x.ToLower());
            Categories = value;
        }
    }

    public List<string> difficulties {
        get { return Difficulties; }
        set {
            value.ForEach(x => x.ToLower());
            Difficulties = value;
        }
    }

    public List<Question> filterAll(List<Question> questions)
    {
        return questions.FindAll(
           q => this.Difficulties.Contains(q.Difficulty.DifficultyName.ToLower())
        );
    }
}
