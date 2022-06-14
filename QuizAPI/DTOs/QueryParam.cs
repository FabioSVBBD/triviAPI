namespace QuizAPI.DTOs;

public class QueryParam
{
    private List<string> categories = new();
    private List<string> difficulties = new();

    public int Page { get; set; } = 1;

    public List<string> Categories { 
        get { return categories; }
        set {
            value.ForEach(c => c.ToLower());
            categories = value;
        }
    }

    public List<string> Difficulties
    {
        get { return difficulties; }
        set
        {
            value.ForEach(c => c.ToLower());
            difficulties = value;
        }
    }
}