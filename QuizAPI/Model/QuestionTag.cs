using System;
using System.Collections.Generic;

namespace QuizAPI.Model
{
    public partial class QuestionTag
    {
        public int TagId { get; set; }
        public int QuestionId { get; set; }
        public int QuestionTagId { get; set; }

        public virtual Question Question { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
