using Dotmim.Sync;
using Dotmim.Sync.SqlServer;
using Dotmim.Sync.Web.Server;
using ISynergy.Framework.Core.Serializers;
using ISynergy.Framework.Synchronization.Factories;

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

var tables = new string[] {
    "Address",
    "Customer",
    "CustomerAddress",
    "ProductCategory",
    "ProductModel",
    "ProductDescription",
    "Product",
    "ProductModelProductDescription"
    };

// [Required] Tables involved in the sync process:
//var setup = new SyncSetup(tables).WithTenantFilter();
var setup = new SyncSetup(tables);

// To add a converter, create an instance and add it to the special WebServerOptions
var webServerOptions = new WebServerOptions();
webServerOptions.SerializerFactories.Add(new MessagePackSerializerFactory());

// add a SqlSyncProvider acting as the server hub
builder.Services.AddSyncServer<SqlSyncChangeTrackingProvider>(connectionString, setup, options, webServerOptions);
//builder.Services.AddSyncServer<SqlSyncProvider>(connectionString, setup, options, webServerOptions);

builder.Services.AddControllers()
    .AddJsonOptions(options => DefaultJsonSerializers.Web());

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseSession();
app.MapControllers();

await app.RunAsync();