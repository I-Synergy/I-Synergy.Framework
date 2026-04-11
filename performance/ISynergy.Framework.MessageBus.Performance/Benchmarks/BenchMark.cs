using BenchmarkDotNet.Attributes;
using ISynergy.Framework.MessageBus.Performance.Models;
using MessagePack;
using MessagePack.Resolvers;
using System.Text.Json;

namespace ISynergy.Framework.MessageBus.Performance.Benchmarks;

/// <summary>
/// Class BenchMark.
/// </summary>
[MemoryDiagnoser]
public class BenchMark
{
    private readonly MessagePackSerializerOptions _options;

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
        StaticCompositeResolver.Instance.Register
        (
            NativeDateTimeResolver.Instance,
            NativeGuidResolver.Instance,
            NativeDecimalResolver.Instance,
            TypelessObjectResolver.Instance,
            StandardResolver.Instance
        );

        _options = MessagePackSerializerOptions.Standard
            .WithResolver(StaticCompositeResolver.Instance)
            .WithCompression(MessagePackCompression.None);

        MessagePackSerializer.DefaultOptions = _options;
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

    /// <summary>
    /// Messages the pack.
    /// </summary>
    [Benchmark]
    public void MessagePack()
    {
        byte[] result = MessagePackSerializer.Serialize(_model, _options);
        MessagePackSerializer.Deserialize<TestModel>(result, _options);
    }
}
