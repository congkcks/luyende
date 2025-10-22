using DeThi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeThi.DTO;
using AutoMapper;
namespace DeThi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class HomeController : Controller
{
    private readonly PostgresContext _context;
    private readonly IMapper _mapper;
    public HomeController(PostgresContext context,IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

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
    //Get all list name test
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<string>>> GetTestList()
    {
        var testTitles = await _context.Tests
            .Select(t => t.Title)
            .ToListAsync();
        return Ok(testTitles);
    }

}

