using System;

namespace ISynergy.Framework.Mvvm.Enumerations
{
    /// <summary>
    /// Enum MessageBoxImage
    /// </summary>
    [Flags]
    public enum MessageBoxImage
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,
        /// <summary>
        /// The hand
        /// </summary>
        Hand = 16,
        /// <summary>
        /// The stop
        /// </summary>
        Stop = 16,
        /// <summary>
        /// The error
        /// </summary>
        Error = 16,
        /// <summary>
        /// The question
        /// </summary>
        Question = 32,
        /// <summary>
        /// The exclamation
        /// </summary>
        Exclamation = 48,
        /// <summary>
        /// The warning
        /// </summary>
        Warning = 48,
        /// <summary>
        /// The asterisk
        /// </summary>
        Asterisk = 64,
        /// <summary>
        /// The information
        /// </summary>
        Information = 64
    }
}
