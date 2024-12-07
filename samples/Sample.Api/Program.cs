using ISynergy.Framework.Core.Serializers;
using Microsoft.EntityFrameworkCore;
using Sample.Api.Data;
using Sample.Api.Entities;

namespace Sample.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
            .AddMvc()
            .AddJsonOptions(options => DefaultJsonSerializers.Default());

        builder.Services.AddDbContext<TestDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        builder.Services.AddControllersWithViews();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApiDocument();

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.UseOpenApi();
        app.UseSwaggerUi();

        // Seed the database
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            for (int i = 1; i <= 1000; i++)
            {
                dbContext.TestEntities.Add(new TestEntity
                {
                    Name = $"Test Entity {i}",
                    Description = $"Description for entity {i}",
                    CreatedDate = DateTime.UtcNow
                });
            }
            dbContext.SaveChanges();
        }


        app.Run();
    }
}
