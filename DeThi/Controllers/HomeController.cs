using DeThi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeThi.DTO;
using AutoMapper;

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

    // ===========================================
    // 🔹 READ - Lấy 1 đề thi theo id
    // GET api/tests/T01
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

    // 🔹 READ - Lấy danh sách tên đề thi
    // GET api/tests/list
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

    // ===========================================
    // 🔹 DELETE - Xóa đề thi
    // DELETE api/tests/T01
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
