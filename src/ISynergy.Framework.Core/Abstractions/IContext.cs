﻿using ISynergy.Framework.Core.Enumerations;

namespace ISynergy.Framework.Core.Abstractions;

/// <summary>
/// Interface IContext
/// </summary>
public interface IContext
{
    IProfile? Profile { get; set; }
    TimeZoneInfo? TimeZone { get; }
    SoftwareEnvironments Environment { get; set; }
    bool IsAuthenticated { get; }
    Dictionary<string, object> Properties { get; }
}
