namespace ISynergy.Framework.UI.Requests;
public record MessageBoxRequest(
    string Message,
    string? Title = "");