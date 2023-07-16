namespace ISynergy.Framework.UI.Controls.ToastNotification.Lifetime
{
    public struct MaximumNotificationCount
    {
        public static MaximumNotificationCount UnlimitedNotifications()
        {
            return new MaximumNotificationCount(int.MaxValue);
        }

        public static MaximumNotificationCount FromCount(int count)
        {
            return new MaximumNotificationCount(count);
        }

        internal int Count { get; }

        private MaximumNotificationCount(int count)
        {
            Count = count;
        }
    }
}
