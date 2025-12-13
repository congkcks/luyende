using DeThi.DTO;
using DeThi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeThi.Controllers;

[ApiController]
[Route("api/test-session")]
public class TestSessionController : ControllerBase
{
    private readonly PostgresContext _context;

    public TestSessionController(PostgresContext context)
    {
        _context = context;
    }

    // ===================== START SESSION =====================
    [HttpPost("start")]
    public async Task<IActionResult> StartSession([FromBody] StartTestSessionDto dto)
    {
        var session = new TestSession
        {
            UserEmail = dto.UserEmail,
            TestId = dto.TestId,
            StartedAt = DateTime.UtcNow
        };

        _context.TestSessions.Add(session);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            sessionId = session.SessionId,
            message = "Session started"
        });
    }

    // ===================== SAVE ANSWERS =====================
    [HttpPost("answers")]
    public async Task<IActionResult> SubmitAnswers([FromBody] List<SubmitAnswerDto> answers)
    {
        if (answers == null || answers.Count == 0)
            return BadRequest("Empty answers list");

        foreach (var dto in answers)
        {
            var existing = await _context.UserAnswers
                .FirstOrDefaultAsync(x =>
                    x.SessionId == dto.SessionId &&
                    x.QuestionId == dto.QuestionId);

            if (existing != null)
            {
                existing.SelectedOption = dto.SelectedOption;
                existing.IsCorrect = dto.IsCorrect;
                existing.AnsweredAt = DateTime.UtcNow;
            }
            else
            {
                _context.UserAnswers.Add(new UserAnswer
                {
                    SessionId = dto.SessionId,
                    QuestionId = dto.QuestionId,
                    SelectedOption = dto.SelectedOption,
                    IsCorrect = dto.IsCorrect,
                    AnsweredAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "All answers saved successfully",
            total = answers.Count
        });
    }

    // ===================== SUBMIT RESULT =====================
    [HttpPost("submit/{sessionId}")]
    public async Task<IActionResult> SubmitResult(
        Guid sessionId,
        [FromBody] SubmitResultDto dto)
    {
        var session = await _context.TestSessions
            .FirstOrDefaultAsync(x => x.SessionId == sessionId);

        if (session == null)
            return NotFound("Session not found");

        session.TotalScore = dto.TotalScore;
        session.FinishedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Result submitted successfully",
            totalScore = session.TotalScore
        });
    }

    // ===================== GET USER SESSIONS =====================
    [HttpGet("user/{userEmail}")]
    public async Task<IActionResult> GetSessionsByUser(string userEmail)
    {
        var sessions = await _context.TestSessions
            .Where(s => s.UserEmail == userEmail)
            .OrderByDescending(s => s.StartedAt)
            .Select(s => new
            {
                s.SessionId,
                s.TestId,
                s.TotalScore,
                s.StartedAt,
                s.FinishedAt,
                duration = s.FinishedAt != null
                    ? (s.FinishedAt.Value - s.StartedAt).ToString()
                    : null
            })
            .ToListAsync();

        return Ok(new
        {
            userEmail,
            totalSessions = sessions.Count,
            data = sessions
        });
    }

    // ===================== REVIEW SESSION =====================
    [HttpGet("review/{sessionId}")]
    public async Task<IActionResult> ReviewSession(Guid sessionId)
    {
        var session = await _context.TestSessions
            .Include(s => s.UserAnswers)
                .ThenInclude(a => a.Question)
                    .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null)
            return NotFound("Session not found");

        return Ok(new
        {
            session.SessionId,
            session.UserEmail,
            session.TestId,
            session.TotalScore,
            session.StartedAt,
            session.FinishedAt,

            answers = session.UserAnswers.Select(a => new
            {
                a.QuestionId,
                a.SelectedOption,
                a.IsCorrect,

                correctAnswer = a.Question.CorrectAnswerLabel,
                explanation = a.Question.AnswerExplanation,

                questionText = a.Question.QuestionText,
                options = a.Question.Options
                    .OrderBy(o => o.OptionLabel)
                    .Select(o => new
                    {
                        label = o.OptionLabel,
                        text = o.OptionText
                    })
            })
        });
    }
}
