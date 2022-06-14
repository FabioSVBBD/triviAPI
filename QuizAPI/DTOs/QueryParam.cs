namespace QuizAPI.DTOs;



public class QueryParam
{
    public List<string> Categories { get; set; } = new();
    public List<string> Difficulties { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public int Page { get; set; } = 1;
}