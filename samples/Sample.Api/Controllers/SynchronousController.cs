using Microsoft.AspNetCore.Mvc;
using Sample.Api.Data;
using Sample.Api.Entities;

namespace Sample.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SynchronousController : ControllerBase
{
    private readonly TestDbContext _context;

    public SynchronousController(TestDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public List<TestEntity> Get()
    {
        return _context.TestEntities
            .OrderByDescending(e => e.CreatedDate)
            .Take(100)
            .ToList();
    }

    [HttpGet("{id}")]
    public TestEntity? GetById(int id)
    {
        return _context.TestEntities.Find(id);
    }

    [HttpPost]
    public ActionResult<TestEntity> Create(TestEntity entity)
    {
        _context.TestEntities.Add(entity);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }
}
