using ISynergy.Framework.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Logging.Base
{
    public class BaseLogger : ILogger
    {
        protected readonly string _name;

        internal IExternalScopeProvider ScopeProvider { get; private set; }

        protected BaseLogger(string name)
        {
            _name = name;
        }

        public virtual IDisposable BeginScope<TState>(TState state) where TState : notnull =>
            NullScope.Instance;

        /// <summary>
        /// If the filter is null, everything is enabled unless the debugger is not attached
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public virtual bool IsEnabled(LogLevel logLevel) => 
            logLevel != LogLevel.None;


        public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;
        }
    }
}
