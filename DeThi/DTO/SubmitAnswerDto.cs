namespace DeThi.DTO;

public class SubmitAnswerDto
{
    public Guid SessionId { get; set; }
    public int QuestionId { get; set; }

    public string SelectedOption { get; set; }
    public bool IsCorrect { get; set; }
}

