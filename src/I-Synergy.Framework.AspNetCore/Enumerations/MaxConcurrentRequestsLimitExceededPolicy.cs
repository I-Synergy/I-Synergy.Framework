namespace ISynergy.Framework.AspNetCore.Enumerations
{
    public enum MaxConcurrentRequestsLimitExceededPolicy
    {
        Drop = 0,
        FifoQueueDropTail = 1,
        FifoQueueDropHead = 2
    }
}
