using System;
using System.Collections.Generic;

namespace DeThi.Models;

public partial class TestSession
{
    public Guid SessionId { get; set; }

    public string? UserEmail { get; set; }

    public string TestId { get; set; } = null!;

    public int TotalScore { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
