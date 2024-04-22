using Dotmim.Sync;
using Dotmim.Sync.SqlServer;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Synchronization;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // [Required]: Handling multiple sessions
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(30));

        // [Required]: Get a connection string to your server data source
        var connectionString = builder.Configuration.GetSection("ConnectionStrings")["SqlConnection"];

        // snapshot directory
        var snapshotDirectoryName = "snapshots";
        var snapshotDirctory = Path.Combine(Environment.CurrentDirectory, snapshotDirectoryName);

        // batches directory
        var batchesDirectoryName = "batches";
        var batchesDirctory = Path.Combine(Environment.CurrentDirectory, batchesDirectoryName);

        var options = new SyncOptions
        {
            SnapshotsDirectory = snapshotDirctory,
            BatchDirectory = batchesDirctory,
            CleanFolder = true,
            CleanMetadatas = true,
            BatchSize = 1000
        };

        // [Required] Tables involved in the sync process:
        //var setup = new SyncSetup(new[] {
        //    "Address",
        //    "Customer",
        //    "CustomerAddress",
        //    "ProductCategory",
        //    "ProductModel",
        //    "ProductDescription",
        //    "Product",
        //    "ProductModelProductDescription"
        //    }).WithTenantFilter();

        var setup = new SyncSetup(new[] {
            "Address",
            "Customer",
            "CustomerAddress",
            "ProductCategory",
            "ProductModel",
            "ProductDescription",
            "Product",
            "ProductModelProductDescription"
            });

        // add a SqlSyncProvider acting as the server hub
        builder.Services.AddSyncServer<SqlSyncProvider>(connectionString, setup, options);

        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseSession();

        app.MapControllers();

        app.Run();
    }
}