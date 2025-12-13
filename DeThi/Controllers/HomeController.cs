using AutoMapper;
using DeThi.DTO;
using DeThi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace DeThi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly PostgresContext _context;
    private readonly IMapper _mapper;

    public HomeController(PostgresContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("start/{testId}")]
    public async Task<IActionResult> StartTest(string testId)
    {
        var test = await _context.Tests
            .Include(t => t.Questions)
                .ThenInclude(q => q.Options)
            .Include(t => t.Questions)
                .ThenInclude(q => q.Group)
            .FirstOrDefaultAsync(t => t.TestId == testId);

        if (test == null)
            return NotFound("Test not found");

        var result = new
        {
            testId = test.TestId,
            title = test.Title,
            duration = test.DurationMinutes,
            totalQuestions = test.TotalQuestions,

            questions = test.Questions
                .OrderBy(q => q.QuestionNumber)
                .Select(q => new
                {
                    questionId = q.QuestionId,
                    questionNumber = q.QuestionNumber,
                    part = q.Part,
                    questionText = q.QuestionText,

                    // ✅ Đáp án đúng
                    correctAnswer = q.CorrectAnswerLabel,

                    // ✅ Giải thích đáp án
                    explanation = q.AnswerExplanation,

                    // ✅ Group (Paragraph / Audio)
                    audioUrl = q.Group != null ? q.Group.AudioUrl : q.AudioUrl,
                    imageUrl = q.Group != null ? q.Group.ImageUrl : q.ImageUrl,
                    passageText = q.Group != null ? q.Group.PassageText : null,

                    options = q.Options
                        .OrderBy(o => o.OptionLabel)
                        .Select(o => new
                        {
                            label = o.OptionLabel,
                            text = o.OptionText
                        })
                })
        };

        return Ok(result);
    }


    [HttpGet("start/{testId}/part/{part}")]
    public async Task<IActionResult> GetByPart(string testId, int part)
    {
        var questions = await _context.Questions
            .Where(q => q.TestId == testId && q.Part == part)
            .Include(q => q.Options)
            .Include(q => q.Group)
            .OrderBy(q => q.QuestionNumber)
            .Select(q => new
            {
                questionId = q.QuestionId,
                questionNumber = q.QuestionNumber,
                part = q.Part,
                questionText = q.QuestionText,

                audioUrl = q.Group != null ? q.Group.AudioUrl : q.AudioUrl,
                imageUrl = q.Group != null ? q.Group.ImageUrl : q.ImageUrl,
                passageText = q.Group != null ? q.Group.PassageText : null,

                options = q.Options.Select(o => new
                {
                    label = o.OptionLabel,
                    text = o.OptionText
                })
            })
            .ToListAsync();

        return Ok(questions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TestDto>> GetTest(string id)
    {
        var test = await _context.Tests
            .Include(t => t.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(t => t.TestId == id);

        if (test == null) return NotFound();

        var dto = _mapper.Map<TestDto>(test);
        return Ok(dto);
    }

    // 🔹 READ - Lấy toàn bộ đề thi
    // GET api/tests
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestDto>>> GetAllTests()
    {
        var tests = await _context.Tests
            .Include(t => t.Questions)
            .ThenInclude(q => q.Options)
            .ToListAsync();

        var dtoList = _mapper.Map<IEnumerable<TestDto>>(tests);
        return Ok(dtoList);
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<object>>> GetTestList()
    {
        var testTitles = await _context.Tests
            .Select(t => new { t.TestId, t.Title })
            .ToListAsync();
        return Ok(testTitles);
    }

    // ===========================================
    // 🔹 CREATE - Thêm đề thi mới
    // POST api/tests
    [HttpPost]
    public async Task<ActionResult<TestDto>> CreateTest([FromBody] TestDto testDto)
    {
        if (testDto == null) return BadRequest("Invalid test data");

        // Map sang entity
        var testEntity = _mapper.Map<Test>(testDto);
        _context.Tests.Add(testEntity);
        await _context.SaveChangesAsync();

        var createdDto = _mapper.Map<TestDto>(testEntity);
        return CreatedAtAction(nameof(GetTest), new { id = testEntity.TestId }, createdDto);
    }

    // ===========================================
    // 🔹 UPDATE - Cập nhật đề thi
    // PUT api/tests/T01
    [HttpPut("{id}")]
    public async Task<ActionResult<TestDto>> UpdateTest(string id, [FromBody] TestDto testDto)
    {
        if (id != testDto.TestId)
            return BadRequest("Test ID mismatch");

        var existingTest = await _context.Tests
            .Include(t => t.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(t => t.TestId == id);

        if (existingTest == null)
            return NotFound();

        // Cập nhật dữ liệu (dùng AutoMapper)
        _mapper.Map(testDto, existingTest);
        await _context.SaveChangesAsync();

        return Ok(_mapper.Map<TestDto>(existingTest));
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTest(string id)
    {
        var test = await _context.Tests
            .Include(t => t.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(t => t.TestId == id);

        if (test == null)
            return NotFound();

        _context.Tests.Remove(test);
        await _context.SaveChangesAsync();

        return NoContent(); // 204
    }

}