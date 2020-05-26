using ISynergy.Framework.Core.Constants;
using ISynergy.Models.General;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.TokenService.Integration.Tests.Fixtures;
using Sample.TokenService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sample.TokenService.Integration.Tests.Basic
{
    [TestClass]
    public class BasicTests
    {
        private WebApplicationFactory<Startup> _factory;
        private PrincipalFixture _fixture;

        [TestInitialize]
        public void InitializeBasicTests()
        {
            _factory = new WebApplicationFactory<Startup>();
            _fixture = new PrincipalFixture();
        }

        [DataTestMethod]
        [DataRow("/swagger", "text/html")]
        public async Task GetEnpointsReturnSuccessAndCorrectContentType(string url, string contentType)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            // Ensure Status code 200-299
            response.EnsureSuccessStatusCode();

            Assert.AreEqual(contentType, response.Content.Headers.ContentType.ToString());
        }

        [TestMethod]
        public async Task PostJwtTokenReturnSuccessAndCorrectContentType()
        {
            // Arrange
            var url = "auth/token/jwt";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync(url, _fixture.TokenRequestFixture);
            //var token = await response.Content.ReadAsAsync<Token>();

            // Assert
            // Ensure Status code 200-299
            response.EnsureSuccessStatusCode();

            Assert.AreEqual(GenericConstants.JsonContentType, response.Content.Headers.ContentType.ToString());
        }

        [TestMethod]
        public async Task PostGetProfileSuccessAndCorrectContentType()
        {
            // Arrange
            var url = "auth/token/wopi";
            var client = _factory.CreateClient();
            var response = await client.PostAsync(url, _fixture.WopiTokenInputToWopiTokenContent());
            var token = await response.Content.ReadAsAsync<WopiToken>();

            // Act
            url = "auth/token/profile";
            response = await client.PostAsync(url, _fixture.TokenToContent(new Token { AccessToken = token.AccessToken }));
            var profile = await response.Content.ReadAsAsync<Profile>();
            
            // Assert
            // Ensure Status code 200-299
            response.EnsureSuccessStatusCode();

            Assert.IsNotNull(profile);
            Assert.IsNotNull(profile?.Token);
            Assert.AreEqual(3, profile?.Roles.Count);
            Assert.AreNotEqual(Guid.Empty, profile?.Account_Id);
            Assert.AreNotEqual(Guid.Empty, profile?.User_Id);
            Assert.AreEqual(GenericConstants.JsonContentType, response.Content.Headers.ContentType.ToString());
        }

        [TestMethod]
        public async Task PostGetWopiTokenSuccessAndCorrectContentType()
        {
            // Arrange
            var url = "auth/token/wopi";
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync(url, _fixture.WopiTokenInputToWopiTokenContent());
            var token = await response.Content.ReadAsAsync<WopiToken>();

            // Assert
            // Ensure Status code 200-299
            response.EnsureSuccessStatusCode();

            Assert.IsNotNull(token);
            Assert.AreEqual(3, token?.Roles.Count);
            Assert.AreNotEqual(Guid.Empty, token?.TenantId);
            Assert.AreNotEqual(Guid.Empty, token?.UserId);
            Assert.AreEqual(GenericConstants.JsonContentType, response.Content.Headers.ContentType.ToString());
        }

        [Ignore("This test is skipped because there are some issues with the serialization of Claim")]
        public async Task PostGetListOfClaimsSuccessAndCorrectContentType()
        {
            // Arrange
            var url = "auth/token/wopi";
            var client = _factory.CreateClient();
            var authresponse = await client.PostAsync(url, _fixture.WopiTokenInputToWopiTokenContent());
            var token = await authresponse.Content.ReadAsAsync<WopiToken>();

            // Assert
            url = $"auth/token/wopi/list";
            var response = await client.PostAsync(url, _fixture.WopiTokenToTokenContent(token));
            var result = await response.Content.ReadAsAsync<List<Claim>>();

            // Assert
            // Ensure Status code 200-299
            response.EnsureSuccessStatusCode();

            Assert.IsNotNull(token);
            Assert.AreEqual(3, token?.Roles.Count);
            Assert.AreNotEqual(Guid.Empty, token?.TenantId);
            Assert.AreNotEqual(Guid.Empty, token?.UserId);
            Assert.AreEqual(GenericConstants.JsonContentType, response.Content.Headers.ContentType.ToString());
        }

        [DataTestMethod]
        [DataRow(ClaimType.RoleType)]
        public async Task PostGetListOfClaimTypeSuccessAndCorrectContentType(string claimType)
        {
            // Arrange
            var url = "auth/token/wopi";
            var client = _factory.CreateClient();
            var authresponse = await client.PostAsync(url, _fixture.WopiTokenInputToWopiTokenContent());
            var token = await authresponse.Content.ReadAsAsync<WopiToken>();

            // Assert
            url = $"auth/token/wopi/{claimType}/list";
            var response = await client.PostAsync(url, _fixture.WopiTokenToTokenContent(token));
            var result = await response.Content.ReadAsAsync<List<string>>();

            // Ensure Status code 200-299
            response.EnsureSuccessStatusCode();

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.ToList().Count);
            Assert.AreEqual(GenericConstants.JsonContentType, response.Content.Headers.ContentType.ToString());
        }

        [DataTestMethod]
        [DataRow(CustomClaimTypes.DocumentIdType)]
        public async Task PostGetSingleOfClaimTypeSuccessAndCorrectContentType(string claimType)
        {
            // Arrange
            var url = "auth/token/wopi";
            var client = _factory.CreateClient();
            var authresponse = await client.PostAsync(url, _fixture.WopiTokenInputToWopiTokenContent());
            var token = await authresponse.Content.ReadAsAsync<WopiToken>();

            // Assert
            url = $"auth/token/wopi/{claimType}/single";
            var response = await client.PostAsync(url, _fixture.WopiTokenToTokenContent(token));
            var result = await response.Content.ReadAsStringAsync();

            // Ensure Status code 200-299
            response.EnsureSuccessStatusCode();

            Assert.IsNotNull(result);
            Assert.AreNotEqual(string.Empty, result);
            Assert.AreEqual(GenericConstants.TextContentType, response.Content.Headers.ContentType.ToString());
        }
    }
}
