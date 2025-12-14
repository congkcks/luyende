using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeThi.Models.English;

public partial class ConversationTemplate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Topic { get; set; } = null!;

    public int Level { get; set; }

    public string Purpose { get; set; } = null!;

    public string JsonContent { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
