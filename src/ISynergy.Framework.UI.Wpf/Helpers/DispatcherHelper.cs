﻿using System.Windows.Threading;

namespace ISynergy.Framework.UI.Helpers
{
    /// <summary>
    /// Dispatcher helper class.
    /// </summary>
    public static class DispatcherHelper
    {
        private static Dispatcher _dispatcher;

        private static readonly DispatcherOperationCallback exitFrameCallback = ExitFrame;

        /// <summary>
        /// Processes all UI messages currently in the message queue.
        /// </summary>
        public static void DoEvents()
        {
            // Create new nested message pump.
            var nestedFrame = new DispatcherFrame();

            // Dispatch a callback to the current message queue, when getting called,
            // this callback will end the nested message loop.
            // note that the priority of this callback should be lower than that of UI event messages.
            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background, exitFrameCallback, nestedFrame);

            // pump the nested message loop, the nested message loop will immediately
            // process the messages left inside the message queue.
            Dispatcher.PushFrame(nestedFrame);

            // If the "exitFrame" callback is not finished, abort it.
            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }

        private static object ExitFrame(object state)
        {
            var frame = state as DispatcherFrame;

            // Exit the nested message loop.
            frame.Continue = false;

            return null;
        }

        /// <summary>
        /// Gets the current dispatcher. This property is compatible with WPF, Silverlight, Windows Phone, etc, and also works
        /// when there is no application object (for example, during unit tests).
        /// </summary>
        /// <value>The current dispatcher.</value>
        public static Dispatcher CurrentDispatcher
        {
            get
            {
                if (_dispatcher == null)
                {
                    _dispatcher = GetCurrentDispatcher();
                }

                return _dispatcher;
            }
        }

        /// <summary>
        /// Gets the current dispatcher.
        /// </summary>
        /// <returns></returns>
        private static Dispatcher GetCurrentDispatcher()
        {
            var currentApplication = System.Windows.Application.Current;

            if (currentApplication != null)
            {
                return currentApplication.Dispatcher;
            }

            return Dispatcher.CurrentDispatcher;
        }
    }
}
