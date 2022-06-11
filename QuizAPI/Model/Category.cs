using System;
using System.Collections.Generic;

namespace QuizAPI.Model
{
    public partial class Category
    {
        public Category()
        {
            Questions = new HashSet<Question>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;

        public virtual ICollection<Question> Questions { get; set; }
    }
}
