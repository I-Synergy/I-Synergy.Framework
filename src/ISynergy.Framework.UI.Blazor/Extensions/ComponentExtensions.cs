﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Windows.Input;

namespace ISynergy.Framework.UI.Extensions;
public static class ComponentExtensions
{
    public static Dictionary<string, object> CommandBinding(this ComponentBase component, ICommand command, object parameter = null, string cssClass = "", bool disabled = false)
    {
        var canExecute = command?.CanExecute(parameter) ?? false;

        return new Dictionary<string, object>
        {
            ["onclick"] = EventCallback.Factory.Create<MouseEventArgs>(component, async (e) =>
            {
                if (command?.CanExecute(parameter) == true)
                    command.Execute(parameter);
            }),
            ["disabled"] = !canExecute || disabled,
            ["class"] = $"{cssClass} {(!canExecute ? "disabled" : "")}"
        };
    }

    public static EventCallback<MouseEventArgs> CommandClick(this ComponentBase component, ICommand command, object parameter = null)
    {
        return EventCallback.Factory.Create<MouseEventArgs>(component, async (e) =>
        {
            if (command?.CanExecute(parameter) == true)
                command.Execute(parameter);
        });
    }
}