
namespace DeThi.Controllers;

using DeThi.Models.English;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ConversationTemplateController : ControllerBase
{
    private readonly EngLishContext _context;

    public ConversationTemplateController(EngLishContext context)
    {
        _context = context;
    }

    // ============================================
    // 1) Lấy toàn bộ template
    // ============================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _context.ConversationTemplates
                                 .OrderByDescending(x => x.CreatedAt)
                                 .ToListAsync();

        return Ok(data);
    }

    // ============================================
    // 2) Lấy theo ID
    // ============================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _context.ConversationTemplates.FindAsync(id);

        if (item == null)
            return NotFound();

        return Ok(item);
    }

    // ============================================
    // 3) Lấy theo Topic + Level (dùng cho FE luyện nói)
    // ============================================
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string topic, [FromQuery] int? level)
    {
        var query = _context.ConversationTemplates.AsQueryable();

        if (!string.IsNullOrWhiteSpace(topic))
            query = query.Where(x => x.Topic.ToLower() == topic.ToLower());

        if (level.HasValue)
            query = query.Where(x => x.Level == level.Value);

        var result = await query.ToListAsync();

        return Ok(result);
    }

    // ============================================
    // 4) Tạo mới template
    // ============================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ConversationTemplate request)
    {
        request.CreatedAt = DateTime.SpecifyKind(request.CreatedAt ?? DateTime.Now, DateTimeKind.Unspecified);


        _context.ConversationTemplates.Add(request);
        await _context.SaveChangesAsync();

        return Ok(request);
    }

    // ============================================
    // 5) Cập nhật template
    // ============================================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ConversationTemplate request)
    {
        var item = await _context.ConversationTemplates.FindAsync(id);
        if (item == null)
            return NotFound();

        item.Topic = request.Topic;
        item.Level = request.Level;
        item.Purpose = request.Purpose;
        item.JsonContent = request.JsonContent;
        item.CreatedAt = item.CreatedAt ?? DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(item);
    }

    // ============================================
    // 6) Xóa template
    // ====================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.ConversationTemplates.FindAsync(id);
        if (item == null)
            return NotFound();

        _context.ConversationTemplates.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Deleted successfully" });
    }
}
