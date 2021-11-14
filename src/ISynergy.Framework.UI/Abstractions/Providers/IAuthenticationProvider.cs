namespace ISynergy.Framework.UI.Abstractions.Providers
{
    /// <summary>
    /// Interface IAuthenticationProvider
    /// </summary>
    public interface IAuthenticationProvider
    {
        /// <summary>
        /// Determines whether this instance [can command be executed] the specified relay command.
        /// </summary>
        /// <param name="Command">The relay command.</param>
        /// <param name="commandParameter">The command parameter.</param>
        /// <returns><c>true</c> if this instance [can command be executed] the specified relay command; otherwise, <c>false</c>.</returns>
        bool CanCommandBeExecuted(ICommand Command, object commandParameter);
        /// <summary>
        /// Determines whether [has access to UI element] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="authorizationTag">The authorization tag.</param>
        /// <returns><c>true</c> if [has access to UI element] [the specified element]; otherwise, <c>false</c>.</returns>
        bool HasAccessToUIElement(object element, object tag, string authorizationTag);
    }
}
