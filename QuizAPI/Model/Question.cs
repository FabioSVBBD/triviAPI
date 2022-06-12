using System;
using System.Collections.Generic;

namespace QuizAPI.Model
{
    public partial class Question
    {
        public Question()
        {
            QuestionTags = new HashSet<QuestionTag>();
        }

        public int QuestionId { get; set; }
        public string Question1 { get; set; } = null!;
        public string Answer { get; set; } = null!;
        public int CategoryId { get; set; }
        public int DifficultyId { get; set; }
        public int StatusId { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual Difficulty Difficulty { get; set; } = null!;
        public virtual Status Status { get; set; } = null!;
        public virtual ICollection<QuestionTag> QuestionTags { get; set; }
    }
}
