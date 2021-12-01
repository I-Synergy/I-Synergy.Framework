using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.AspNetCore.Synchronization
{
    public static partial class HttpServerSyncEventsId
    {
        public static EventId HttpGettingRequest => new EventId(30100, nameof(HttpGettingRequest));
        public static EventId HttpSendingResponse => new EventId(30150, nameof(HttpSendingResponse));
        public static EventId HttpSendingChanges => new EventId(30000, nameof(HttpSendingChanges));
        public static EventId HttpGettingChanges => new EventId(30050, nameof(HttpGettingChanges));
    }
}
