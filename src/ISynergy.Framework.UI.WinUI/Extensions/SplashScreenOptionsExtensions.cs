using ISynergy.Framework.UI.Configuration;
using Windows.Media.Core;
using Windows.Storage.Streams;

namespace ISynergy.Framework.UI.Extensions;
public static class SplashScreenOptionsExtensions
{
    public static MediaSource CreateMediaSource(this SplashScreenOptions configuration, Stream stream)
    {
        var randomAccessStream = new InMemoryRandomAccessStream();
        var outputStream = randomAccessStream.GetOutputStreamAt(0);
        var dataWriter = new DataWriter(outputStream);

        var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        dataWriter.WriteBytes(memoryStream.ToArray());

        dataWriter.StoreAsync().GetAwaiter().GetResult();
        outputStream.FlushAsync().GetAwaiter().GetResult();

        return MediaSource.CreateFromStream(randomAccessStream, configuration.ContentType);
    }
}
