using ISynergy.Models.General;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ISynergy.Common
{
    public static class ClaimConverters
    {
        public static UserInfo ConvertClaimsToUserInfo(IEnumerable<System.Security.Claims.Claim> claims)
        {
            var result = new UserInfo()
            {
                Account_Id = Guid.Parse(claims.Where(q => q.Type == ClaimTypes.AccountIdType).Select(q => q.Value).Single()),
                Account_Description = claims.Where(q => q.Type == ClaimTypes.AccountDescriptionType).Select(q => q.Value).Single(),
                TimeZoneId = claims.Where(q => q.Type == ClaimTypes.TimeZoneType).Select(q => q.Value).Single(),
                User_Id = Guid.Parse(claims.Where(q => q.Type == ClaimTypes.UserIdType).Select(q => q.Value).Single()),
                RfidUid = int.Parse(claims.Where(q => q.Type == ClaimTypes.RfidUidType).Select(q => q.Value).Single()),
                Username = claims.Where(q => q.Type == ClaimTypes.UserNameType).Select(q => q.Value).Single(),
                Email = claims.Where(q => q.Type == ClaimTypes.UserNameType).Select(q => q.Value).Single(),
                License_Expration = DateTimeOffset.Parse(claims.Where(q => q.Type == ClaimTypes.LicenseExprationType).Select(q => q.Value).Single(), CultureInfo.InvariantCulture),
                License_Users = int.Parse(claims.Where(q => q.Type == ClaimTypes.LicenseUsersType).Select(q => q.Value).Single()),
                Roles = claims.Where(q => q.Type == ClaimTypes.RoleType).Select(q => q.Value).ToList(),
                Modules = claims.Where(q => q.Type == ClaimTypes.ModulesType).Select(q => q.Value).ToList()
            };

            return result;
        }
    }
}
