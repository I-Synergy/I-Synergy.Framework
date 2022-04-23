using Flurl;
using ISynergy.Framework.Core.Constants;
using ISynergy.Models.General;
using System.Text.Json;
using Sample.TokenService;
using Sample.TokenService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sample.TokenService.Integration.Tests.Fixtures
{
    public class PrincipalFixture
    {
        public ByteArrayContent TokenRequestFixture { get; }

        public PrincipalFixture()
        {
            var request = new TokenRequest
            {
                Username = "test@demo.com",
                Claims = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(CustomClaimTypes.ApplicationIdType, Guid.NewGuid().ToString()),
                    new KeyValuePair<string, string>(CustomClaimTypes.DocumentIdType, Guid.NewGuid().ToString()),
                    new KeyValuePair<string, string>(ClaimType.AccountIdType, Guid.NewGuid().ToString()),
                    new KeyValuePair<string, string>(ClaimType.UserIdType, Guid.NewGuid().ToString())
                },
                Roles = new List<string>
                {
                    OfficeOnlineDocumentModes.Edit,
                    OfficeOnlineDocumentModes.EditNew,
                    OfficeOnlineDocumentModes.View
                },
                Expiration = TimeSpan.FromMinutes(1)
            };

            var body = JsonSerializer.Serialize(request);
            var buffer = System.Text.Encoding.UTF8.GetBytes(body);

            TokenRequestFixture = new ByteArrayContent(buffer);
            TokenRequestFixture.Headers.ContentType = new MediaTypeHeaderValue(GenericConstants.JsonMediaType);
        }

        public ByteArrayContent TokenToContent(Token token)
        {
            var body = JsonSerializer.Serialize(token);
            var buffer = System.Text.Encoding.UTF8.GetBytes(body);

            var result = new ByteArrayContent(buffer);
            result.Headers.ContentType = new MediaTypeHeaderValue(GenericConstants.JsonMediaType);

            return result;
        }

        public ByteArrayContent WopiTokenInputToWopiTokenContent()
        {
            var wopiToken = new WopiTokenInput(
                "ApplicationId",
                Guid.NewGuid(),
                Guid.NewGuid(),
                "test@demo.com",
                Guid.NewGuid(),
                "Brandname",
                "BrandUrl",
                "FriendlyName",
                new List<string> { "A", "B", "C"},
                TimeSpan.FromMinutes(1));

            var body = JsonSerializer.Serialize(wopiToken);
            var buffer = System.Text.Encoding.UTF8.GetBytes(body);

            var result = new ByteArrayContent(buffer);
            result.Headers.ContentType = new MediaTypeHeaderValue(GenericConstants.JsonMediaType);

            return result;
        }

        public ByteArrayContent WopiTokenToTokenContent(WopiToken token)
        {
            var body = JsonSerializer.Serialize(new Token { AccessToken = token.AccessToken });
            var buffer = System.Text.Encoding.UTF8.GetBytes(body);

            var result = new ByteArrayContent(buffer);
            result.Headers.ContentType = new MediaTypeHeaderValue(GenericConstants.JsonMediaType);

            return result;
        }
    }
}
