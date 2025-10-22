namespace DeThi.DTO;

public class TestDto
{
    public string TestId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public List<QuestionDto> Questions { get; set; } = new();
}