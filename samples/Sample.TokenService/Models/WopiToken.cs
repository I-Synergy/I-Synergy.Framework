using System;
using System.Collections.Generic;
using ISynergy.Framework.Core.Validation;

namespace Sample.TokenService.Models
{
    public class WopiTokenInput
    {
        public string ApplicationId { get; }
        public Guid TenantId { get; }
        public Guid UserId { get; }
        public string Username { get; }
        public Guid DocumentId { get; }
        public string BrandName { get; }
        public string BrandUrl { get; }
        public string FriendlyName { get; }
        public List<string> Roles { get; }
        public TimeSpan Expiration { get; }

        public WopiTokenInput(
            string applicationId,
            Guid tenantId,
            Guid userId,
            string username,
            Guid documentId,
            string brandName,
            string brandUrl,
            string friendlyName,
            List<string> roles,
            TimeSpan expiration)
        {
            Argument.IsNotNullOrEmpty(nameof(applicationId), applicationId);
            Argument.IsNotNullOrEmpty(nameof(tenantId), tenantId);
            Argument.IsNotNullOrEmpty(nameof(userId), userId);
            Argument.IsNotNullOrEmpty(nameof(username), username);
            Argument.IsNotNullOrEmpty(nameof(documentId), documentId);
            Argument.IsNotNullOrEmpty(nameof(brandName), brandName);
            Argument.IsNotNullOrEmpty(nameof(brandUrl), brandUrl);
            Argument.IsNotNullOrEmpty(nameof(friendlyName), friendlyName);
            Argument.IsNotNull(nameof(expiration), expiration);

            ApplicationId = applicationId;
            TenantId = tenantId;
            UserId = userId;
            Username = username;
            DocumentId = documentId;
            BrandName = brandName;
            BrandUrl = brandUrl;
            FriendlyName = friendlyName;
            Roles = roles;
            Expiration = expiration;
        }
    }

    public class WopiToken : WopiTokenInput
    {
        public WopiToken(
            string applicationId,
            Guid tenantId,
            Guid userId,
            string username,
            Guid documentId,
            string brandName,
            string brandUrl,
            string friendlyName,
            List<string> roles,
            TimeSpan expiration,
            string accessToken)
            : base(applicationId, tenantId, userId, username, documentId, brandName, brandUrl, friendlyName, roles, expiration)
        {
            Argument.IsNotNullOrEmpty(nameof(accessToken), accessToken);

            AccessToken = accessToken;
        }

        public string AccessToken { get; }
        public DateTime ExpirationUtc { get; }
    }
}
