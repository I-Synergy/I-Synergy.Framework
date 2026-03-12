// Polyfill to enable C# records/init-only properties on netstandard2.0
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
