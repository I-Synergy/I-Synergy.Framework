using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Globalization.Constants;
using ISynergy.Framework.Globalization.Converters;
using System.Runtime.CompilerServices;

// The types below now physically live in the ISynergy.Framework.Core assembly, which is what lets
// Core drop its project reference to Globalization. These forwarders preserve the binary identity
// of the moved types for consumers compiled against the original ISynergy.Framework.Globalization
// assembly.
[assembly: TypeForwardedTo(typeof(ILanguageService))]
[assembly: TypeForwardedTo(typeof(GlobalizationConstants))]
[assembly: TypeForwardedTo(typeof(DateTimeConverter))]
[assembly: TypeForwardedTo(typeof(DateTimeOffsetConverter))]
[assembly: TypeForwardedTo(typeof(TimeOnlyConverter))]
