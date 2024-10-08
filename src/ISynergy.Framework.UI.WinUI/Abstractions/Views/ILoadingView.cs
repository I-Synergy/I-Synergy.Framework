﻿using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Mvvm.Abstractions;
using Microsoft.UI.Xaml;

namespace ISynergy.Framework.UI.Abstractions.Views;

public interface ILoadingView : IView
{
    /// <summary>
    /// Handles the application initialized message.
    /// </summary>
    /// <param name="message"></param>
    void ApplicationInitialized(ApplicationInitializedMessage message);
    /// <summary>
    /// The loadingview should have an signin button that triggers this event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void SignInClicked(object sender, RoutedEventArgs e);
}
