using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions
{
    [TestClass]
    public class ExceptionExtensionsTests
    {
        [TestMethod]
        public void ToMessage_ReturnsCorrectLogMessage()
        {
            string? logMessage = null;

            // Arrange
            try
            {
                throw new Exception("Test Exception");
            }
            catch (Exception ex)
            {
                logMessage = ex.ToMessage(Environment.StackTrace);

                Assert.IsTrue(logMessage.Contains(ex.Message));
                Assert.IsNotNull(Environment.StackTrace);
                Assert.IsTrue(logMessage.Contains(Environment.StackTrace));
            }
        }

        [TestMethod]
        public void ToMessage_ReturnsLogMessageWithoutEnvironmentStackTrace()
        {
            // Arrange
            var exception = new Exception("Test Exception");
            var environmentStackTrace = "";

            // Act
            var logMessage = exception.ToMessage(environmentStackTrace);

            // Assert
            Assert.IsTrue(logMessage.Contains(exception.Message));
            Assert.IsTrue(string.IsNullOrEmpty(environmentStackTrace));
            Assert.IsTrue(logMessage.Contains(environmentStackTrace));
        }

        [TestMethod]
        public void ToMessage_ReturnsLogMessageWithNullEnvironmentStackTrace()
        {
            // Arrange
            var exception = new Exception("Test Exception");
            string? environmentStackTrace = null;

            // Act
            var logMessage = exception.ToMessage(environmentStackTrace!);

            // Assert
            Assert.IsTrue(logMessage.Contains(exception.Message));
            Assert.IsTrue(string.IsNullOrEmpty(environmentStackTrace));
        }

        #region SanitizeSensitiveData

        [TestMethod]
        public void SanitizeSensitiveData_NullInput_ReturnsNull()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData(null!);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SanitizeSensitiveData_EmptyInput_ReturnsEmpty()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData(string.Empty);
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void SanitizeSensitiveData_NoSensitiveData_ReturnsUnchanged()
        {
            const string input = "This is a normal error message with no secrets.";
            var result = ExceptionExtensions.SanitizeSensitiveData(input);
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void SanitizeSensitiveData_InputExceedsMaxLength_ReturnsUnchanged()
        {
            // Build an input larger than MaxSanitizeInputLength (64,000)
            var sb = new System.Text.StringBuilder(65_000);
            sb.Append('x', 65_000);
            // Embed a connection string that would normally be redacted
            sb.Append("ConnectionString=my-secret-db-connection");
            var input = sb.ToString();

            var result = ExceptionExtensions.SanitizeSensitiveData(input);

            // Should be returned unchanged — too large to process
            Assert.AreEqual(input, result);
            Assert.IsTrue(result.Contains("my-secret-db-connection"),
                "Large input should skip sanitization entirely, leaving sensitive data intact.");
        }

        [TestMethod]
        public void SanitizeSensitiveData_InputJustUnderMaxLength_StillSanitizes()
        {
            // ~1000 chars is well under the 64,000 limit
            var padding = new string('x', 1000);
            var input = $"ConnectionString=my-secret-db-connection {padding}";

            var result = ExceptionExtensions.SanitizeSensitiveData(input);

            Assert.IsFalse(result.Contains("my-secret-db-connection"),
                "Input under the limit should still be sanitized.");
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        #region Connection Strings

        [TestMethod]
        public void SanitizeSensitiveData_ConnectionString_RedactsValue()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData(
                "ConnectionString=Server=localhost;Database=mydb;User ID=admin;Password=secret123");
            Assert.IsFalse(result.Contains("localhost"));
            Assert.IsFalse(result.Contains("mydb"));
            Assert.IsFalse(result.Contains("admin"));
            Assert.IsFalse(result.Contains("secret123"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_DataSource_RedactsValue()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData(
                "Data Source=prod-server.example.com;Initial Catalog=MyDb");
            Assert.IsFalse(result.Contains("prod-server"));
            Assert.IsFalse(result.Contains("MyDb"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_ServerKeyword_RedactsValue()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData("Server=db.internal:5432");
            Assert.IsFalse(result.Contains("db.internal"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_UserId_RedactsValue()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData("User ID=firstname.lastname");
            Assert.IsFalse(result.Contains("firstname.lastname"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_IntegratedSecurity_RedactsValue()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData("Integrated Security=True;Trusted_Connection=Yes");
            Assert.IsFalse(result.Contains("True"));
            Assert.IsFalse(result.Contains("Yes"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_ServerName_NotPartialMatch()
        {
            // "ServerName" should NOT be redacted — it's not "Server" with a word boundary before =
            const string input = "The ServerName property was not set correctly.";
            var result = ExceptionExtensions.SanitizeSensitiveData(input);
            Assert.AreEqual(input, result,
                "\"ServerName\" (no =) should not be mistaken for \"Server=\".");
        }

        #endregion

        #region Passwords and Tokens

        [TestMethod]
        public void SanitizeSensitiveData_PasswordEquals_RedactsValue()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData("password=MyP@ssw0rd!");
            Assert.IsFalse(result.Contains("MyP@ssw0rd!"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_PasswordColon_RedactsValue()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData("password: my-secret-password");
            Assert.IsFalse(result.Contains("my-secret-password"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_Token_RedactsValue()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData("token=abc123def456ghi789");
            Assert.IsFalse(result.Contains("abc123def456ghi789"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_BearerToken_RedactsValue()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData("bearer=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.xxx.yyy");
            Assert.IsFalse(result.Contains("eyJhbGci"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_ApiKey_RedactsValue()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData("apikey=sk-1234567890abcdef");
            Assert.IsFalse(result.Contains("sk-1234567890abcdef"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_Secret_RedactsValue()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData("secret=my-client-secret-value");
            Assert.IsFalse(result.Contains("my-client-secret-value"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_QuotedPassword_RedactsValue()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData("password=\"quoted secret\"");
            Assert.IsFalse(result.Contains("quoted secret"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_PassInPassed_NotPartialMatch()
        {
            // "passed" should not trigger "pass=" because of \b boundary
            const string input = "The test passed successfully.";
            var result = ExceptionExtensions.SanitizeSensitiveData(input);
            Assert.AreEqual(input, result,
                "\"passed\" (word boundary) should not be mistaken for \"pass=\".");
        }

        [TestMethod]
        public void SanitizeSensitiveData_KeyInKeyword_NotPartialMatch()
        {
            // "keyword" should not trigger "key="
            const string input = "This is a keyword search result.";
            var result = ExceptionExtensions.SanitizeSensitiveData(input);
            Assert.AreEqual(input, result,
                "\"keyword\" should not be mistaken for \"key=\".");
        }

        #endregion

        #region API Keys

        [TestMethod]
        public void SanitizeSensitiveData_ApiKeyBase64_RedactsValue()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData(
                "api_key=dGhpcyBpcyBhIHRlc3Qgc2VjcmV0IGtleSBmb3IgdW5pdCB0ZXN0aW5n");
            Assert.IsFalse(result.Contains("dGhpcyBpcyBhIHRlc3Q"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_SecretKey_RedactsValue()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData(
                "secret_key=A1b2C3d4E5f6G7h8I9j0K1l2M3n4O5p6Q7r8S9t0==");
            Assert.IsFalse(result.Contains("A1b2C3d4E5f6G7h8"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_PrivateKey_RedactsValue()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData(
                "private_key=MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCtest1234567890abcdef==");
            Assert.IsFalse(result.Contains("MIGfMA0GCSqGSIb3DQEBAQU"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_ShortBase64_NotRedacted()
        {
            // Base64 under 20 chars should not match the API key pattern (min 20)
            const string input = "api_key=short1234567890";
            var result = ExceptionExtensions.SanitizeSensitiveData(input);
            Assert.AreEqual(input, result,
                "API key values shorter than 20 chars should not be redacted by the base64 pattern.");
        }

        #endregion

        #region JWT Tokens

        [TestMethod]
        public void SanitizeSensitiveData_JwtToken_RedactsEntireToken()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData(
                "Authorization: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c");
            Assert.IsFalse(result.Contains("eyJhbGci"));
            Assert.IsFalse(result.Contains("eyJzdWIi"));
            Assert.IsFalse(result.Contains("SflKxw"));
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_MultipleJwtTokens_RedactsAll()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData(
                "Token1: eyJhbGciOiJSUzI1NiJ9.eyJ1c2VyIjoiYWRtaW4ifQ.signature1 " +
                "Token2: eyJ0eXAiOiJKV1QifQ.eyJyb2xlIjoidXNlciJ9.signature2");
            Assert.IsFalse(result.Contains("eyJhbGciOiJSUzI1NiJ9"));
            Assert.IsFalse(result.Contains("eyJ0eXAiOiJKV1QifQ"));
            Assert.AreEqual(2, CountOccurrences(result, "[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_MalformedJwt_NotRedacted()
        {
            // A JWT needs exactly two dots separating three segments
            const string input = "eyJhbGciOiJIUzI1NiJ9-no-second-segment";
            var result = ExceptionExtensions.SanitizeSensitiveData(input);
            // The "eyJ" prefix is there but it's not a valid JWT format (only one dot)
            // The JWT pattern requires exactly two dots with valid segments
            Assert.IsTrue(result.Contains("eyJhbGci"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_EyJWithoutDots_NotRedacted()
        {
            // "eyJ" followed by chars but no dots — should not be treated as JWT
            // This is the exact scenario that caused the backtracking timeout
            var input = "Some base64-like data: eyJ" + new string('A', 500) + " more text";
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var result = ExceptionExtensions.SanitizeSensitiveData(input);
            sw.Stop();

            // Should complete very quickly (well under the 100ms regex timeout)
            Assert.IsTrue(sw.ElapsedMilliseconds < 90,
                $"Sanitization took {sw.ElapsedMilliseconds}ms — should be well under the 100ms timeout.");
            // The string should be returned unchanged for the base64 part
            Assert.IsTrue(result.Contains("eyJ"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_EyJWithOnlyOneDot_NotRedacted()
        {
            // "eyJ" with one dot but not two — another backtracking risk scenario.
            // IMPORTANT: avoid leading "token:" / "bearer:" etc. — those get caught
            // by the password/token pattern. Use a neutral prefix instead.
            var input = "Some data: eyJ" + new string('B', 300) + "." + new string('C', 300);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var result = ExceptionExtensions.SanitizeSensitiveData(input);
            sw.Stop();

            Assert.IsTrue(sw.ElapsedMilliseconds < 90,
                $"Sanitization took {sw.ElapsedMilliseconds}ms — should be well under the 100ms timeout.");
            // Not a valid JWT (only one dot), so eyJ should remain
            Assert.IsTrue(result.Contains("eyJ"));
        }

        #endregion

        #region File Paths

        [TestMethod]
        public void SanitizeSensitiveData_WindowsPath_RedactsUsername()
        {
            var input = @"Error accessing C:\Users\testuser\Documents\config.json";
            var result = ExceptionExtensions.SanitizeSensitiveData(input);
            Assert.IsFalse(result.Contains("testuser"));
            // The callback preserves "C:" (the colon triggers key:value logic) then redacts the rest
            Assert.IsTrue(result.Contains(@"C: [REDACTED]"));
            Assert.IsTrue(result.Contains(@"\Documents\config.json"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_UnixHomePath_RedactsUsername()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData(
                "File not found: /home/testuser/config.yml");
            Assert.IsFalse(result.Contains("testuser"));
            // No colon or equals in the match, so the whole match becomes [REDACTED]
            Assert.IsTrue(result.Contains("[REDACTED]/config.yml"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_UnixUsersPath_RedactsUsername()
        {
            var result = ExceptionExtensions.SanitizeSensitiveData(
                "Error in /Users/testuser/.bashrc");
            Assert.IsFalse(result.Contains("testuser"));
            // No colon or equals in the match, so the whole match becomes [REDACTED]
            Assert.IsTrue(result.Contains("[REDACTED]/.bashrc"));
        }

        #endregion

        #region Combined Scenarios

        [TestMethod]
        public void SanitizeSensitiveData_MultiplePatternsInOneString_RedactsAll()
        {
            var testPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
            var input = "Connection failed: Data Source=myserver;User ID=admin;Password=s3cret " +
                        $"using token=ghp_1234567890abcdef at {testPath}";
            var result = ExceptionExtensions.SanitizeSensitiveData(input);

            Assert.IsFalse(result.Contains("myserver"));
            Assert.IsFalse(result.Contains("admin"));
            Assert.IsFalse(result.Contains("s3cret"));
            Assert.IsFalse(result.Contains("ghp_1234567890abcdef"));
            // Multiple redactions (connection string values, password, token)
            Assert.IsTrue(CountOccurrences(result, "[REDACTED]") >= 3);
        }

        [TestMethod]
        public void SanitizeSensitiveData_RealWorldExceptionMessage_RedactsSensitiveParts()
        {
            // Simulates a realistic exception message from a database connection failure
            var input = """
                Exception type: System.Data.SqlClient.SqlException
                Message: Cannot open database "MyAppDb" requested by the login. The login failed.
                Login failed for user 'sa'.
                ConnectionString: Data Source=prod-db.internal;Initial Catalog=MyAppDb;User ID=sa;Password=SuperSecret123!
                """;

            var result = ExceptionExtensions.SanitizeSensitiveData(input);

            Assert.IsFalse(result.Contains("SuperSecret123!"));
            Assert.IsFalse(result.Contains("prod-db.internal"));
            // "sa" in key=value positions (User ID=sa, Password=...) should be redacted
            Assert.IsTrue(result.Contains("[REDACTED]"));
        }

        [TestMethod]
        public void SanitizeSensitiveData_StackOverflowException_HandlesLongMessage()
        {
            // Simulates a very long repetitive message that could stress the regex
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Stack overflow in recursive method:");
            for (int i = 0; i < 500; i++)
            {
                sb.AppendLine($"  at MyApp.Services.NestedService.RecursiveCall({i})");
            }

            var sw = System.Diagnostics.Stopwatch.StartNew();
            var result = ExceptionExtensions.SanitizeSensitiveData(sb.ToString());
            sw.Stop();

            // Should complete well within the timeout
            Assert.IsTrue(sw.ElapsedMilliseconds < 90,
                $"Sanitization took {sw.ElapsedMilliseconds}ms — should be well under the 100ms timeout.");
            Assert.IsNotNull(result);
        }

        #endregion

        #region Helpers

        private static int CountOccurrences(string text, string substring)
        {
            int count = 0;
            int index = 0;
            while ((index = text.IndexOf(substring, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += substring.Length;
            }
            return count;
        }

        #endregion

        #endregion
    }
}
