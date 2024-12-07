using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample.Api.Data;
using Sample.Api.Entities;

namespace Sample.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BadAsyncController : ControllerBase
{
    private readonly TestDbContext _context;

    public BadAsyncController(TestDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public List<TestEntity> Get()
    {
        return _context.TestEntities
            .OrderByDescending(e => e.CreatedDate)
            .Take(100)
            .ToListAsync()
            .Result;
    }

    [HttpGet("{id}")]
    public TestEntity GetById(int id)
    {
        return _context.TestEntities.FindAsync(id).Result;
    }

    [HttpPost]
    public ActionResult<TestEntity> Create(TestEntity entity)
    {
        _context.TestEntities.Add(entity);
        _context.SaveChangesAsync().GetAwaiter().GetResult();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }
}
