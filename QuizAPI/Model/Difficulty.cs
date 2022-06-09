using System;
using System.Collections.Generic;

namespace QuizAPI.Model
{
    public partial class Difficulty
    {
        public Difficulty()
        {
            Questions = new HashSet<Question>();
        }

        public int DifficultyId { get; set; }
        public string DifficultyName { get; set; } = null!;

        public virtual ICollection<Question> Questions { get; set; }
    }
}
