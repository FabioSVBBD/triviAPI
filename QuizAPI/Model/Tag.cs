﻿using System;
using System.Collections.Generic;

namespace QuizAPI.Model
{
    public partial class Tag
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = null!;
    }
}
