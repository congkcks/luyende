using System;
using System.Collections.Generic;

namespace DeThi.Models;

public partial class QuestionGroup
{
    public string GroupId { get; set; } = null!;

    public string TestId { get; set; } = null!;

    public int Part { get; set; }

    public string? PassageText { get; set; }

    public string? AudioUrl { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual Test Test { get; set; } = null!;
}
