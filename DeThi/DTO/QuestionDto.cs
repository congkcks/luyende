namespace DeThi.DTO;

public class QuestionDto
{
    public int QuestionId { get; set; }
    public int QuestionNumber { get; set; }
    public string QuestionText { get; set; } = null!;
    public string? QuestionType { get; set; }
    public string? AudioUrl { get; set; }
    public string? ImageUrl { get; set; }
    public string CorrectAnswerLabel { get; set; } = null!;
    public string? AnswerExplanation { get; set; }
    public List<OptionDto> Options { get; set; } = new();
}