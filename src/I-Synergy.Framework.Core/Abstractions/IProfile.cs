using System;
using System.Collections.Generic;
using ISynergy.Framework.Core.Models;

namespace ISynergy.Framework.Core.Abstractions
{
    public interface IProfile
    {
        Guid AccountId { get; }
        string AccountDescription { get; }
        string TimeZoneId { get; }
        Guid UserId { get; }
        string Username { get; }
        string Email { get; }
        List<string> Roles { get; }
        List<string> Modules { get; }
        DateTimeOffset LicenseExpration { get; }
        int LicenseUsers { get; }
        Token Token { get; }
        DateTime Expiration { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
    }
}
