using System;
using System.Collections.Generic;

namespace DeThi.Models;

public partial class UserAnswer
{
    public int AnswerId { get; set; }

    public Guid SessionId { get; set; }

    public int UserId { get; set; }

    public int QuestionId { get; set; }

    public string SelectedOption { get; set; } = null!;

    public bool? IsCorrect { get; set; }

    public DateTime? AnsweredAt { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual TestSession Session { get; set; } = null!;
}
