using BenchmarkDotNet.Attributes;
using ISynergy.Framework.MessageBus.Performance.Models;
using System.Text.Json;

namespace ISynergy.Framework.MessageBus.Performance.Benchmarks;

/// <summary>
/// Class BenchMark.
/// </summary>
[MemoryDiagnoser]
public class BenchMark
{
    /// <summary>
    /// Gets or sets the model.
    /// </summary>
    /// <value>The model.</value>
    private TestModel? _model { get; set; } // NOSONAR - initialized by GlobalSetup in derived usage


    /// <summary>
    /// Initializes a new instance of the <see cref="BenchMark"/> class.
    /// </summary>
    public BenchMark()
    {
    }

    /// <summary>
    /// Gets the test object.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>TestModel.</returns>
    public TestModel GetTestObject(byte[] file)
    {
        TestModel result = new()
        {
            Id = Guid.NewGuid(),
            Description = "Description",
            Number = 1,
            Data = file,
            Exception = new ArgumentException($"Test exception")
        };

        return result;
    }

    /// <summary>
    /// Jsons this instance.
    /// </summary>
    [Benchmark(Baseline = true)]
    public void Json()
    {
        string result = JsonSerializer.Serialize(_model);
        JsonSerializer.Deserialize<TestModel>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}
