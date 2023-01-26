namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class TaskExtensions.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Awaits the specified task.
        /// </summary>
        /// <param name="task">The self.</param>
        public static void Await(this Task task) => task.AwaitTask();

        /// <summary>
        /// Awaits the specified task.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        public static void Await<T>(this Task<T> task) => task.AwaitTask();

        /// <summary>
        /// Awaits the specified task with error action.
        /// </summary>
        /// <param name="task">The self.</param>
        /// <param name="completedAction">The error action.</param>
        public static void Await(this Task task, Action completedAction = null) => task.AwaitTask(completedAction);

        /// <summary>
        /// Awaits the specified task with error action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="completedAction"></param>
        public static void Await<T>(this Task<T> task, Action completedAction = null) => task.AwaitTask(completedAction);

        /// <summary>
        /// Awaits the specified task with competed and error action.
        /// </summary>
        /// <param name="task">The self.</param>
        /// <param name="completedAction"></param>
        /// <param name="errorAction">The error action.</param>
        public static void Await(this Task task, Action completedAction = null, Action<Exception> errorAction = null) => task.AwaitTask(completedAction, errorAction);

        /// <summary>
        /// Awaits the specified task with competed and error action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="completedAction"></param>
        /// <param name="errorAction"></param>
        public static void Await<T>(this Task<T> task, Action completedAction = null, Action<Exception> errorAction = null) => task.AwaitTask(completedAction, errorAction);

        /// <summary>
        /// Awaits the specified task with competed and error action.
        /// </summary>
        /// <param name="task">The self.</param>
        /// <param name="completedAction">The completed action.</param>
        /// <param name="errorAction">The error action.</param>
        private static void AwaitTask(this Task task, Action completedAction = null, Action<Exception> errorAction = null)
        {
            try
            {
                task.GetAwaiter().GetResult();
                completedAction?.Invoke();
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex);
            }
        }

        /// <summary>
        /// Awaits the specified task with competed and error action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="completedAction"></param>
        /// <param name="errorAction"></param>
        /// <returns></returns>
        private static T AwaitTask<T>(this Task<T> task, Action completedAction = null, Action<Exception> errorAction = null)
        {
            var result = default(T);
            
            try
            {
                result = task.GetAwaiter().GetResult();
                completedAction?.Invoke();
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex);
            }

            return result;
        }
    }
}
