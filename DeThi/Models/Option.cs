using System;
using System.Collections.Generic;

namespace DeThi.Models;

public partial class Option
{
    public int OptionId { get; set; }

    public int? QuestionId { get; set; }

    public string OptionLabel { get; set; } = null!;

    public string? OptionText { get; set; }

    public virtual Question? Question { get; set; }
}
