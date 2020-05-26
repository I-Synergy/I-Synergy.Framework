using System;
using System.IO;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Imaging;

namespace ISynergy.Framework.Windows.Helpers
{
    /// <summary>
    /// Class ResourceHelper.
    /// </summary>
    internal static class ResourceHelper
    {
        /// <summary>
        /// Gets the resource dictionary by path.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="resourcePath">The resource path.</param>
        /// <returns>ResourceDictionary.</returns>
        public static ResourceDictionary GetResourceDictionaryByPath(Type type, string resourcePath)
        {
            var assembly = type.GetTypeInfo().Assembly;
            using var stream = assembly.GetManifestResourceStream(resourcePath);
            var reader = new StreamReader(stream);
            var dictionary = XamlReader.Load(reader.ReadToEnd()) as ResourceDictionary;
            return dictionary;
        }

        /// <summary>
        /// Loads the embedded resource.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="resourcePath">The resource path.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.Object.</returns>
        public static object LoadEmbeddedResource(Type type, string resourcePath, object key)
        {
            return GetResourceDictionaryByPath(type, resourcePath)[key];
        }

        /// <summary>
        /// Loads the manifest stream bytes.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="resourcePath">The resource path.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] LoadManifestStreamBytes(Type type, string resourcePath)
        {
            var assembly = type.GetTypeInfo().Assembly;
            using var stream = assembly.GetManifestResourceStream(resourcePath);
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            return bytes;
        }

        /// <summary>
        /// Loads the embedded image resource.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="resourcePath">The resource path.</param>
        /// <returns>BitmapImage.</returns>
        public static BitmapImage LoadEmbeddedImageResource(Type type, string resourcePath)
        {
            var assembly = type.GetTypeInfo().Assembly;
            using var stream = assembly.GetManifestResourceStream(resourcePath);
            var buffer = new MemoryStream();
            stream.CopyTo(buffer);
            buffer.Seek(0, SeekOrigin.Begin);

            var image = new BitmapImage();
            image.SetSource(buffer.AsRandomAccessStream());

            return image;
        }
    }
}
