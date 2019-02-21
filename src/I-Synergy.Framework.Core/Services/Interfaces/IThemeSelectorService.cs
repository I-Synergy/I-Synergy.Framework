﻿using System;

namespace ISynergy.Services
{
    public interface IThemeSelectorService
    {
        bool IsLightThemeEnabled { get; }
        object Theme { get; set; }

        event EventHandler<object> OnThemeChanged;

        void Initialize();
        void SetRequestedTheme();
        void SetTheme(object theme);
        void SwitchTheme();
    }
}