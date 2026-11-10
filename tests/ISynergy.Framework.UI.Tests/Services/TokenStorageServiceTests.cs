using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Tests for <see cref="TokenStorageService"/>.
/// </summary>
/// <remarks>
/// The test project targets a non-platform TFM, so the secure-storage primitives compile down to the
/// no-op <c>#else</c> branch (read returns <c>null</c>, writes/deletes do nothing). That makes this the
/// right place to pin the platform-independent behaviour: the contract of the getters, the guard
/// clauses in <see cref="TokenStorageService.SaveTokensAsync"/>, and the exception-filter logging
/// pattern (<c>catch (Exception ex) when (LogSaveTokensFailure(ex))</c>) that logs and rethrows.
/// </remarks>
[TestClass]
public class TokenStorageServiceTests
{
    private Mock<ILogger<TokenStorageService>> _logger = null!;
    private TokenStorageService _service = null!;

    [TestInitialize]
    public void Initialize()
    {
        _logger = new Mock<ILogger<TokenStorageService>>();
        _service = new TokenStorageService(_logger.Object);
    }

    #region Getters

    [TestMethod]
    public async Task GetAccessTokenAsync_ReturnsNull_WhenNoSecureStoreIsAvailable()
    {
        Assert.IsNull(await _service.GetAccessTokenAsync());
    }

    [TestMethod]
    public async Task GetRefreshTokenAsync_ReturnsNull_WhenNoSecureStoreIsAvailable()
    {
        Assert.IsNull(await _service.GetRefreshTokenAsync());
    }

    [TestMethod]
    public async Task GetIdTokenAsync_ReturnsNull_WhenNoSecureStoreIsAvailable()
    {
        Assert.IsNull(await _service.GetIdTokenAsync());
    }

    [TestMethod]
    public async Task GetTokenExpiryAsync_ReturnsNull_WhenNoSecureStoreIsAvailable()
    {
        Assert.IsNull(await _service.GetTokenExpiryAsync());
    }

    [TestMethod]
    public async Task GetEnvironmentAsync_ReturnsNull_WhenNoSecureStoreIsAvailable()
    {
        Assert.IsNull(await _service.GetEnvironmentAsync());
    }

    #endregion

    #region SaveTokensAsync

    [TestMethod]
    public async Task SaveTokensAsync_WithFullTokenSet_LogsDebug()
    {
        // Act
        await _service.SaveTokensAsync(
            "access",
            "refresh",
            "id",
            DateTimeOffset.UtcNow,
            "Production");

        // Assert
        VerifyLogged(LogLevel.Debug, "Tokens saved successfully");
        VerifyNeverLoggedError();
    }

    [TestMethod]
    public async Task SaveTokensAsync_WithNoValues_SkipsWritesAndLogsDebug()
    {
        // Act - all guard clauses take the "nothing to store" branch
        await _service.SaveTokensAsync(null, null, null, null, null!);

        // Assert
        VerifyLogged(LogLevel.Debug, "Tokens saved successfully");
        VerifyNeverLoggedError();
    }

    [TestMethod]
    public async Task SaveTokensAsync_WithEmptyStrings_SkipsWrites()
    {
        // Act
        await _service.SaveTokensAsync(string.Empty, string.Empty, string.Empty, null, string.Empty);

        // Assert
        VerifyLogged(LogLevel.Debug, "Tokens saved successfully");
    }

    [TestMethod]
    public async Task SaveTokensAsync_WhenStorageFails_LogsErrorAndRethrowsOriginalException()
    {
        // Arrange - the platform write is a no-op here, so make the debug log throw to exercise
        // the exception-filter path.
        const string failureMessage = "secure storage unavailable";
        MakeLoggerThrowForLevel(LogLevel.Debug, failureMessage);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.SaveTokensAsync("access", "refresh", "id", DateTimeOffset.UtcNow, "Production"));

        // Assert - original exception is preserved and the failure was logged as an error
        Assert.AreEqual(failureMessage, exception.Message);
        VerifyLoggedWithException(LogLevel.Error, "Failed to save tokens", exception);
    }

    #endregion

    #region ClearTokensAsync

    [TestMethod]
    public async Task ClearTokensAsync_LogsDebug()
    {
        // Act
        await _service.ClearTokensAsync();

        // Assert
        VerifyLogged(LogLevel.Debug, "Tokens cleared successfully");
        VerifyNeverLoggedError();
    }

    [TestMethod]
    public async Task ClearTokensAsync_WhenStorageFails_LogsErrorAndRethrowsOriginalException()
    {
        // Arrange
        const string failureMessage = "secure storage unavailable";
        MakeLoggerThrowForLevel(LogLevel.Debug, failureMessage);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ClearTokensAsync());

        // Assert
        Assert.AreEqual(failureMessage, exception.Message);
        VerifyLoggedWithException(LogLevel.Error, "Failed to clear tokens", exception);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Makes the logger's <see cref="ILogger.Log{TState}"/> throw for the given level only, so the
    /// error log written from the exception filter still succeeds.
    /// </summary>
    private void MakeLoggerThrowForLevel(LogLevel level, string message)
    {
        _logger
            .Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(invocation =>
            {
                if ((LogLevel?)invocation.Arguments[0] == level)
                    throw new InvalidOperationException(message);
            }));
    }

    private void VerifyLogged(LogLevel level, string message) =>
        _logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state!.ToString() == message),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

    /// <summary>
    /// Verifies that the level/message were logged with the exact same exception instance, which is
    /// how the exception-filter helpers keep the original exception intact.
    /// </summary>
    private void VerifyLoggedWithException(LogLevel level, string message, Exception exception) =>
        _logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state!.ToString() == message),
                It.Is<Exception?>(e => ReferenceEquals(e, exception)),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

    private void VerifyNeverLoggedError() =>
        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);

    #endregion
}
