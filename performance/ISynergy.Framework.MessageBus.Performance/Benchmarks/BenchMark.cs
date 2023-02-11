using BenchmarkDotNet.Attributes;
using ISynergy.Framework.MessageBus.Performance.Models;
using MessagePack;
using MessagePack.Resolvers;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ISynergy.Framework.MessageBus.Performance.Benchmarks
{
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
        public TestModel Model { get; set; }

        /// <summary>
        /// The file name
        /// </summary>
        [Params("file.docx", "file.pdf", "file.jpg", "file.json", "file.xlsx", "file.zip")]
        public string fileName;

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
            var result = new TestModel
            {
                Id = Guid.NewGuid(),
                Description = "Description",
                Number = 1,
                Data = file,
                Exception = new ArgumentException($"Test exception")
            };

            return result;
        }

        //[GlobalSetup]
        //public async Task GlobalSetup()
        //{
        //    var file = await File.ReadAllBytesAsync(Path.Combine(Environment.CurrentDirectory, "Data", fileName));
        //    var bytes = SerializeToByteArray(GetTestObject(file));

        //    using MemoryStream memoryStream = new MemoryStream();
        //    using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress))
        //    {
        //        zipStream.Write(bytes, 0, bytes.Length);
        //        zipStream.Close();

        //        _model = memoryStream.ToArray();
        //    }
        //}

        /// <summary>
        /// Globals the setup.
        /// </summary>
        [GlobalSetup]
        public async Task GlobalSetup()
        {
            var file = await File.ReadAllBytesAsync(Path.Combine(Environment.CurrentDirectory, "Data", fileName));
            Model = GetTestObject(file);
        }

        //private byte[] SerializeToByteArray(object obj)
        //{
        //    if (obj is null)
        //    {
        //        return null;
        //    }
        //    var bf = new BinaryFormatter();
        //    using (var ms = new MemoryStream())
        //    {
        //        bf.Serialize(ms, obj);
        //        return ms.ToArray();
        //    }
        //}

        //private T Deserialize<T>(byte[] byteArray) where T : class
        //{
        //    if (byteArray is null)
        //    {
        //        return null;
        //    }
        //    using (var memStream = new MemoryStream())
        //    {
        //        var binForm = new BinaryFormatter();
        //        memStream.Write(byteArray, 0, byteArray.Length);
        //        memStream.Seek(0, SeekOrigin.Begin);
        //        var obj = (T)binForm.Deserialize(memStream);
        //        return obj;
        //    }
        //}


        /// <summary>
        /// Jsons this instance.
        /// </summary>
        [Benchmark(Baseline = true)]
        public void Json()
        {
            var result = JsonSerializer.Serialize(Model);
            JsonSerializer.Deserialize<TestModel>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        /// <summary>
        /// Binaries the formatter.
        /// </summary>
        //[Benchmark]
        //public void BinaryFormatter()
        //{
        //    byte[] result = null;

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        BinaryFormatter serializer = new BinaryFormatter();
        //        serializer.Serialize(ms, Model);
        //        result = ms.ToArray();
        //    }

        //    using (MemoryStream ms2 = new MemoryStream(result))
        //    {
        //        BinaryFormatter serializer = new BinaryFormatter();
        //        serializer.Deserialize(ms2);
        //    };
        //}

        ///// <summary>
        ///// Bsons this instance.
        ///// </summary>
        //[Benchmark]
        //public void Bson()
        //{
        //    byte[] result = null;

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        using (BsonDataWriter writer = new BsonDataWriter(ms))
        //        {
        //            JsonSerializer serializer = new JsonSerializer();
        //            serializer.Serialize(writer, Model);
        //        }

        //        result = ms.ToArray();
        //    }

        //    using (MemoryStream ms2 = new MemoryStream(result))
        //    {
        //        using (BsonDataReader reader = new BsonDataReader(ms2))
        //        {
        //            JsonSerializer serializer = new JsonSerializer();
        //            serializer.Deserialize(reader);
        //        }
        //    };
        //}

        /// <summary>
        /// Messages the pack.
        /// </summary>
        [Benchmark]
        public void MessagePack()
        {
            var result = MessagePackSerializer.Serialize(Model, _options);
            MessagePackSerializer.Deserialize<TestModel>(result, _options);
        }
    }
}
