using System;
using System.Collections.Generic;

namespace DeThi.Models;

public partial class TestSession
{
    public Guid SessionId { get; set; }

    public Guid UserId { get; set; }

    public string TestId { get; set; } = null!;

    public int? ListeningCorrect { get; set; }

    public int? ReadingCorrect { get; set; }

    public int? TotalCorrect { get; set; }

    public int? ToeicScore { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    public virtual Test Test { get; set; } = null!;

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
