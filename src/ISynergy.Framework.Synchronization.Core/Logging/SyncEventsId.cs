using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Synchronization.Core
{
    public static partial class SyncEventsId
    {
        private static EventId CreateEventId(int id, string eventName) => new EventId(id, eventName);

        public static EventId Exception => CreateEventId(0, nameof(Exception));
        public static EventId ReportProgress => CreateEventId(5, nameof(ReportProgress));
        public static EventId Interceptor => CreateEventId(10, nameof(Interceptor));
    }
}
