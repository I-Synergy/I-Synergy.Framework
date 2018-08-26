using System.Collections.Generic;

namespace ISynergy
{
    public class ClaimTypes
    {
        //Used by identity token
        public const string AccountIdType = "account_id";
        public const string AccountDescriptionType = "account_description";
        public const string UserNameType = "username";
        public const string UserIdType = "user_id";
        public const string ClientIdType = "client_id";
        public const string RfidUidType = "rfid_uid";
        public const string RoleType = "role";
        public const string ModulesType = "modules";
        public const string LicenseExprationType = "license_expration";
        public const string LicenseUsersType = "license_users";
        public const string SecurityStampType = "security_stamp";
        public const string TimeZoneType = "timezone";

        //Used by role-based claims
        public const string PermissionType = "permission";

        public static IEnumerable<string> AuthorizationCodeClaimTypes =>
            new[] { UserIdType, UserNameType, SecurityStampType };

        public static IEnumerable<string> AccessTokenClaimTypes
            => new[] { AccountIdType, AccountDescriptionType, UserNameType, UserIdType, RfidUidType, RoleType, ModulesType, LicenseExprationType, LicenseUsersType };

        public static IEnumerable<string> RefreshTokenClaimTypes
            => new[] { UserNameType, UserIdType, RfidUidType, SecurityStampType };

        public static IEnumerable<string> IntrospectClaimTypes =>
            new[] { AccountIdType };
    }
}
