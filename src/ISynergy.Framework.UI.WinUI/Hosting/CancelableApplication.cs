﻿using Microsoft.UI.Xaml;

namespace ISynergy.Framework.UI.Hosting;

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

    private bool _isClosing = false;

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