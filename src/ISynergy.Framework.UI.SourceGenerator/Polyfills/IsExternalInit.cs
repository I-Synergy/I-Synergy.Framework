// Polyfill to enable C# records/init-only properties on netstandard2.0
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { } // NOSONAR - required polyfill for C# 9 record/init-only property support
}
