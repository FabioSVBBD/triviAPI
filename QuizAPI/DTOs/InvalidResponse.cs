namespace QuizAPI.DTOs
{
    public class InvalidResponseDTO
    {
        private string message;
        private List<string> possibleResponses;

        public InvalidResponseDTO(string message, List<string> possibleResponses)
        {
            this.message = message;
            this.possibleResponses = possibleResponses;
        }

        public string Message { get => message; set => message = value; }
        public List<string> PossibleResponses { get => possibleResponses; set => possibleResponses = value; }
    }
}