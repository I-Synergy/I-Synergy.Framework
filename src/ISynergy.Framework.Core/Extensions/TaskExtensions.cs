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
        /// <param name="_self">The self.</param>
        public async static void Await(this Task _self)
        {
            try
            {
                await _self;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Awaits the specified error action.
        /// </summary>
        /// <param name="_self">The self.</param>
        /// <param name="errorAction">The error action.</param>
        public async static void Await(this Task _self, Action<Exception> errorAction)
        {
            try
            {
                await _self;
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex);
            }
        }

        /// <summary>
        /// Awaits the specified completed action.
        /// </summary>
        /// <param name="_self">The self.</param>
        /// <param name="completedAction">The completed action.</param>
        /// <param name="errorAction">The error action.</param>
        public async static void Await(this Task _self, Action completedAction, Action<Exception> errorAction)
        {
            try
            {
                await _self;
                completedAction?.Invoke();
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex);
            }
        }
    }
}
