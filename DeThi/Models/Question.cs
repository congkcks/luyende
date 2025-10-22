using System;
using System.Collections.Generic;

namespace DeThi.Models;

public partial class Question
{
    public int QuestionId { get; set; }

    public string? TestId { get; set; }

    public int QuestionNumber { get; set; }

    public string QuestionType { get; set; } = null!;

    public string? QuestionText { get; set; }

    public string? QuestionGroupId { get; set; }

    public string? AudioUrl { get; set; }

    public string? ImageUrl { get; set; }

    public string CorrectAnswerLabel { get; set; } = null!;

    public string? AnswerExplanation { get; set; }

    public virtual ICollection<Option> Options { get; set; } = new List<Option>();

    public virtual Test? Test { get; set; }
}
