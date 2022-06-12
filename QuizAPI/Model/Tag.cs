using System;
using System.Collections.Generic;

namespace QuizAPI.Model
{
    public partial class Tag
    {
        public Tag()
        {
            QuestionTags = new HashSet<QuestionTag>();
        }

        public int TagId { get; set; }
        public string TagName { get; set; } = null!;

        public virtual ICollection<QuestionTag> QuestionTags { get; set; }
    }
}
