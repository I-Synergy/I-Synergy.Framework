using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample.Api.Data;
using Sample.Api.Entities;

namespace Sample.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AsynchronousController : ControllerBase
{
    private readonly TestDbContext _context;

    public AsynchronousController(TestDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public Task<List<TestEntity>> Get()
    {
        return _context.TestEntities
            .OrderByDescending(e => e.CreatedDate)
            .Take(100)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<TestEntity> GetById(int id)
    {
        return await _context.TestEntities.FindAsync(id);
    }

    [HttpPost]
    public async Task<ActionResult<TestEntity>> Create(TestEntity entity)
    {
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }
}
