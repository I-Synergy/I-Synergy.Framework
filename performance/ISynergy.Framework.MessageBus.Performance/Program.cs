using BenchmarkDotNet.Running;
using ISynergy.Framework.MessageBus.Performance.Benchmarks;
using System;

namespace ISynergy.Framework.MessageBus.Performance;

/// <summary>
/// Class Program.
/// </summary>
class Program
{
    /// <summary>
    /// When long file paths is not enabled you get the "Invalid runtimeconfig.json in .NET Benchmark" error.
    /// Solve this with the following:https://www.howtogeek.com/266621/how-to-make-windows-10-accept-file-paths-over-260-characters/
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        //Manual test for saving to file.
        //var bm = new BenchMarks.BenchMark();
        //var file = await File.ReadAllBytesAsync(Path.Combine(Environment.CurrentDirectory, "Data", "file.zip"));
        //bm.Model = bm.GetTestObject(file);
        //bm.MessagePack();

        BenchmarkRunner.Run<BenchMark>();
        Console.ReadLine();
    }
}
