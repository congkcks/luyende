using DeThi.Models;
using DeThi.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace DeThi.Controllers;

[ApiController]
[Route("api/test-session")]
public class TestSessionController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public TestSessionController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // ============================
    // 1. Bắt đầu làm bài → tạo session
    // POST api/test-session/start
    [HttpPost("start")]
    public async Task<IActionResult> StartSession([FromBody] StartTestSessionDto dto)
    {
        var session = new TestSession
        {
            UserId = dto.UserId,
            TestId = dto.TestId,
            StartedAt = DateTime.UtcNow
        };

        _context.TestSessions.Add(session);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            sessionId = session.SessionId,
            message = "Session started successfully"
        });
    }

    // ============================
    // 2. Lưu đáp án từng câu
    // POST api/test-session/answer
    [HttpPost("answer")]
    public async Task<IActionResult> SubmitAnswer([FromBody] SubmitAnswerDto dto)
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

        await _context.SaveChangesAsync();

        return Ok(new { message = "Answer saved" });
    }

    // ============================
    // 3. Nộp bài + lưu điểm
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

        session.ListeningCorrect = dto.ListeningCorrect;
        session.ReadingCorrect = dto.ReadingCorrect;
        session.TotalCorrect = dto.TotalCorrect;
        session.ToeicScore = dto.ToeicScore;
        session.FinishedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Result submitted successfully" });
    }

    // ============================
    // 4. Xem lại bài đã làm
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
            return NotFound();

        return Ok(session);
    }

    // ============================
    // 5. Lấy lịch sử làm bài của 1 user
    // GET api/test-session/history/{userId}
    [HttpGet("history/{userId}")]
    public async Task<IActionResult> GetHistory(Guid userId)
    {
        var history = await _context.TestSessions
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync();

        return Ok(history);
    }
}
