﻿using System.Windows.Input;

namespace ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;

public interface IKeyboardEventHandler
{
    void Handle(KeyEventArgs eventArgs);
}
