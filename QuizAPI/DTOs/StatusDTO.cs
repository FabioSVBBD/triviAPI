using QuizAPI.Model;

namespace QuizAPI.DTOs
{
    public class StatusDTO
    {
        private string? status;

		public StatusDTO(String status)
		{
			this.status = status;
		}

		public string? Status { get => status; set => status = value; }

		public static StatusDTO? AsDTO(String status)
		{
			if (status == null)
			{
				return null;
			}

			return new StatusDTO(status);
		}
	}
}
