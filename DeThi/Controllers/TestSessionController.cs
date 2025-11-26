using DeThi.DTO;
using DeThi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

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

    // ============================
    // 1. Bắt đầu làm bài → tạo session
    // POST api/test-session/start
    [HttpPost("start")]
    public async Task<IActionResult> StartSession([FromBody] StartTestSessionDto dto)
    {
        var session = new TestSession
        {
            UserId = dto.UserId,          // int từ service khác
            UserEmail = dto.UserEmail,    // optional
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

    // ============================
    // 2. Lưu danh sách đáp án (1 lần)
    // POST api/test-session/answers
    [HttpPost("answers")]
    public async Task<IActionResult> SubmitAnswers([FromBody] List<SubmitAnswerDto> answers)
    {
        if (answers == null || answers.Count == 0)
            return BadRequest("Empty answers list");

        var sessionId = answers.First().SessionId;

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
                var answer = new UserAnswer
                {
                    SessionId = dto.SessionId,
                    UserId = dto.UserId,
                    QuestionId = dto.QuestionId,
                    SelectedOption = dto.SelectedOption,
                    IsCorrect = dto.IsCorrect,
                    AnsweredAt = DateTime.UtcNow
                };

                _context.UserAnswers.Add(answer);
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "All answers saved successfully",
            total = answers.Count
        });
    }


    // ============================
    // 3. Nộp bài → cập nhật điểm tổng
    // POST api/test-session/submit/{sessionId}
    [HttpPost("submit/{sessionId}")]
    public async Task<IActionResult> SubmitResult(
        Guid sessionId,
        [FromBody] SubmitResultDto dto)
    {
        var session = await _context.TestSessions
            .FirstOrDefaultAsync(x => x.SessionId == sessionId);

        if (session == null)
            return NotFound("Session not found");

        session.TotalScore = dto.TotalScore;   // ✅ chỉ còn 1 điểm tổng
        session.FinishedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Result submitted successfully",
            totalScore = session.TotalScore
        });
    }

    // ============================
    // 4. Review lại bài đã làm
    // GET api/test-session/review/{sessionId}
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
            sessionId = session.SessionId,
            userId = session.UserId,
            testId = session.TestId,
            totalScore = session.TotalScore,
            startedAt = session.StartedAt,
            finishedAt = session.FinishedAt,

            answers = session.UserAnswers.Select(a => new
            {
                questionId = a.QuestionId,
                selectedOption = a.SelectedOption,
                isCorrect = a.IsCorrect,

                correctAnswer = a.Question.CorrectAnswerLabel,
                explanation = a.Question.AnswerExplanation,

                questionText = a.Question.QuestionText,
                options = a.Question.Options
                    .OrderBy(o => o.OptionLabel)
                    .Select(o => new {
                        label = o.OptionLabel,
                        text = o.OptionText
                    })
            })
        });
    }

    // ============================
    // 5. Lấy lịch sử làm bài của user
    // GET api/test-session/history/{userId}
    [HttpGet("history/{userId:int}")]
    public async Task<IActionResult> GetHistory(int userId)
    {
        var history = await _context.TestSessions
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.StartedAt)
            .Select(x => new
            {
                x.SessionId,
                x.TestId,
                x.TotalScore,
                x.StartedAt,
                x.FinishedAt
            })
            .ToListAsync();

        return Ok(history);
    }
}
