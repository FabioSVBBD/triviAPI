using System;
using System.Collections.Generic;

namespace QuizAPI.Model
{
    public partial class Status
    {
        public Status()
        {
            Questions = new HashSet<Question>();
        }

        public int StatusId { get; set; }
        public string StatusName { get; set; } = null!;

        public virtual ICollection<Question> Questions { get; set; }
    }
}
