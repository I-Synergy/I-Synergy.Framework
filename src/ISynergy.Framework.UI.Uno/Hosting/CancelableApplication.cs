﻿#if NET7_0_WINDOWS10_0_19041 && !HAS_UNO
using Microsoft.UI.Xaml;

namespace ISynergy.Framework.UI.Hosting
{
    public class CancelableApplication : Application
    {
        public IServiceProvider Services
        {
            get; internal set;
        }

        public CancellationToken Token
        {
            get; internal set;
        }

        public bool _isClosing;

        protected void ExitSuccess()
        {
            if (_isClosing)
            {
                return;
            }

            Exiting?.Invoke();
            _isClosing = true;
        }


        public event Action Exiting;
    }
}
#endif