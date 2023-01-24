using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class TaskExtensions.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Awaits the specified self.
        /// </summary>
        /// <param name="task">The self.</param>
        public static void Await(this Task task) => task.AwaitTask();

        /// <summary>
        /// Awaits the specified error action.
        /// </summary>
        /// <param name="task">The self.</param>
        /// <param name="errorAction">The error action.</param>
        public static void Await(this Task task, Action<Exception> errorAction = null) => task.AwaitTask(errorAction: errorAction);

        /// <summary>
        /// Awaits the specified error action.
        /// </summary>
        /// <param name="task">The self.</param>
        /// <param name="completedAction"></param>
        /// <param name="errorAction">The error action.</param>
        public static void Await(this Task task, Action completedAction = null, Action<Exception> errorAction = null) => task.AwaitTask(completedAction, errorAction);

        /// <summary>
        /// Awaits the specified completed action.
        /// </summary>
        /// <param name="task">The self.</param>
        /// <param name="completedAction">The completed action.</param>
        /// <param name="errorAction">The error action.</param>
        private async static void AwaitTask(this Task task, Action completedAction = null, Action<Exception> errorAction = null)
        {
            try
            {
                await task;
                completedAction?.Invoke();
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex);
            }
        }
    }
}
