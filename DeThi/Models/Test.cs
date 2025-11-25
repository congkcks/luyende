using System;
using System.Collections.Generic;

namespace DeThi.Models;

public partial class Test
{
    public string TestId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int TotalQuestions { get; set; }

    public int? DurationMinutes { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<QuestionGroup> QuestionGroups { get; set; } = new List<QuestionGroup>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<TestSession> TestSessions { get; set; } = new List<TestSession>();
}
