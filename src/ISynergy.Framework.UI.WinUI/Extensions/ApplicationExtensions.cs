using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.UI.Styles;
using Microsoft.UI.Xaml;

namespace ISynergy.Framework.UI.Extensions;
public static class ApplicationExtensions
{
    public static Microsoft.UI.Xaml.Application SetApplicationColor(this Microsoft.UI.Xaml.Application application, string color)
    {
        foreach (var item in application.Resources.MergedDictionaries.EnsureNotNull())
        {
            if (!item.Source.AbsoluteUri.EndsWith("themeresources.xaml") &&
                !item.Source.AbsoluteUri.EndsWith("themeresources_v2.xaml") &&
                !item.Source.AbsoluteUri.EndsWith("Images.xaml") &&
                !item.Source.AbsoluteUri.EndsWith("Style.xaml") &&
                !item.Source.AbsoluteUri.EndsWith($"Theme{color.Substring(1, 6)}"))
                application.Resources.MergedDictionaries.Remove(item);
        }

        switch (color)
        {
            case "#ff8c00":
                var style_ff8c00 = new Themeff8c00();
                if (!application.Resources.MergedDictionaries.Contains(style_ff8c00))
                    application.Resources.MergedDictionaries.Add(style_ff8c00);
                break;
            case "#f7630c":
                var style_f7630c = new Themef7630c();
                if (!application.Resources.MergedDictionaries.Contains(style_f7630c))
                    application.Resources.MergedDictionaries.Add(style_f7630c);
                break;
            case "#ca5010":
                var style_ca5010 = new Themeca5010();
                if (!application.Resources.MergedDictionaries.Contains(style_ca5010))
                    application.Resources.MergedDictionaries.Add(style_ca5010);
                break;
            case "#da3b01":
                var style_da3b01 = new Themeda3b01();
                if (!application.Resources.MergedDictionaries.Contains(style_da3b01))
                    application.Resources.MergedDictionaries.Add(style_da3b01);
                break;
            case "#ef6950":
                var style_ef6950 = new Themeef6950();
                if (!application.Resources.MergedDictionaries.Contains(style_ef6950))
                    application.Resources.MergedDictionaries.Add(style_ef6950);
                break;
            case "#d13438":
                var style_d13438 = new Themed13438();
                if (!application.Resources.MergedDictionaries.Contains(style_d13438))
                    application.Resources.MergedDictionaries.Add(style_d13438);
                break;
            case "#ff4343":
                var style_ff4343 = new Themeff4343();
                if (!application.Resources.MergedDictionaries.Contains(style_ff4343))
                    application.Resources.MergedDictionaries.Add(style_ff4343);
                break;
            case "#e74856":
                var style_e74856 = new Themee74856();
                if (!application.Resources.MergedDictionaries.Contains(style_e74856))
                    application.Resources.MergedDictionaries.Add(style_e74856);
                break;
            case "#e81123":
                var style_e81123 = new Themee81123();
                if (!application.Resources.MergedDictionaries.Contains(style_e81123))
                    application.Resources.MergedDictionaries.Add(style_e81123);
                break;
            case "#ea005e":
                var style_ea005e = new Themeea005e();
                if (!application.Resources.MergedDictionaries.Contains(style_ea005e))
                    application.Resources.MergedDictionaries.Add(style_ea005e);
                break;
            case "#c30052":
                var style_c30052 = new Themec30052();
                if (!application.Resources.MergedDictionaries.Contains(style_c30052))
                    application.Resources.MergedDictionaries.Add(style_c30052);
                break;
            case "#e3008c":
                var style_e3008c = new Themee3008c();
                if (!application.Resources.MergedDictionaries.Contains(style_e3008c))
                    application.Resources.MergedDictionaries.Add(style_e3008c);
                break;
            case "#bf0077":
                var style_bf0077 = new Themebf0077();
                if (!application.Resources.MergedDictionaries.Contains(style_bf0077))
                    application.Resources.MergedDictionaries.Add(style_bf0077);
                break;
            case "#c239b3":
                var style_c239b3 = new Themec239b3();
                if (!application.Resources.MergedDictionaries.Contains(style_c239b3))
                    application.Resources.MergedDictionaries.Add(style_c239b3);
                break;
            case "#9a0089":
                var style_9a0089 = new Theme9a0089();
                if (!application.Resources.MergedDictionaries.Contains(style_9a0089))
                    application.Resources.MergedDictionaries.Add(style_9a0089);
                break;
            case "#0078d7":
                var style_0078d7 = new Theme0078d7();
                if (!application.Resources.MergedDictionaries.Contains(style_0078d7))
                    application.Resources.MergedDictionaries.Add(style_0078d7);
                break;
            case "#0063b1":
                var style_0063b1 = new Theme0063b1();
                if (!application.Resources.MergedDictionaries.Contains(style_0063b1))
                    application.Resources.MergedDictionaries.Add(style_0063b1);
                break;
            case "#8e8cd8":
                var style_8e8cd80 = new Theme8e8cd8();
                if (!application.Resources.MergedDictionaries.Contains(style_8e8cd80))
                    application.Resources.MergedDictionaries.Add(style_8e8cd80);
                break;
            case "#6b69d6":
                var style_6b69d6 = new Theme6b69d6();
                if (!application.Resources.MergedDictionaries.Contains(style_6b69d6))
                    application.Resources.MergedDictionaries.Add(style_6b69d6);
                break;
            case "#8764b8":
                var style_8764b8 = new Theme8764b8();
                if (!application.Resources.MergedDictionaries.Contains(style_8764b8))
                    application.Resources.MergedDictionaries.Add(style_8764b8);
                break;
            case "#744da9":
                var style_744da9 = new Theme744da9();
                if (!application.Resources.MergedDictionaries.Contains(style_744da9))
                    application.Resources.MergedDictionaries.Add(style_744da9);
                break;
            case "#b146c2":
                var style_b146c2 = new Themeb146c2();
                if (!application.Resources.MergedDictionaries.Contains(style_b146c2))
                    application.Resources.MergedDictionaries.Add(style_b146c2);
                break;
            case "#881798":
                var style_881798 = new Theme881798();
                if (!application.Resources.MergedDictionaries.Contains(style_881798))
                    application.Resources.MergedDictionaries.Add(style_881798);
                break;
            case "#0099bc":
                var style_0099bc = new Theme0099bc();
                if (!application.Resources.MergedDictionaries.Contains(style_0099bc))
                    application.Resources.MergedDictionaries.Add(style_0099bc);
                break;
            case "#2d7d9a":
                var style_2d7d9a = new Theme2d7d9a();
                if (!application.Resources.MergedDictionaries.Contains(style_2d7d9a))
                    application.Resources.MergedDictionaries.Add(style_2d7d9a);
                break;
            case "#00b7c3":
                var style_00b7c3 = new Theme00b7c3();
                if (!application.Resources.MergedDictionaries.Contains(style_00b7c3))
                    application.Resources.MergedDictionaries.Add(style_00b7c3);
                break;
            case "#038387":
                var style_038387 = new Theme038387();
                if (!application.Resources.MergedDictionaries.Contains(style_038387))
                    application.Resources.MergedDictionaries.Add(style_038387);
                break;
            case "#00b294":
                var style_00b294 = new Theme00b294();
                if (!application.Resources.MergedDictionaries.Contains(style_00b294))
                    application.Resources.MergedDictionaries.Add(style_00b294);
                break;
            case "#018574":
                var style_018574 = new Theme018574();
                if (!application.Resources.MergedDictionaries.Contains(style_018574))
                    application.Resources.MergedDictionaries.Add(style_018574);
                break;
            case "#00cc6a":
                var style_00cc6a = new Theme00cc6a();
                if (!application.Resources.MergedDictionaries.Contains(style_00cc6a))
                    application.Resources.MergedDictionaries.Add(style_00cc6a);
                break;
            case "#10893e":
                var style_10893e = new Theme10893e();
                if (!application.Resources.MergedDictionaries.Contains(style_10893e))
                    application.Resources.MergedDictionaries.Add(style_10893e);
                break;
            case "#7a7574":
                var style_7a7574 = new Theme7a7574();
                if (!application.Resources.MergedDictionaries.Contains(style_7a7574))
                    application.Resources.MergedDictionaries.Add(style_7a7574);
                break;
            case "#5d5a58":
                var style_5d5a58 = new Theme5d5a58();
                if (!application.Resources.MergedDictionaries.Contains(style_5d5a58))
                    application.Resources.MergedDictionaries.Add(style_5d5a58);
                break;
            case "#68768a":
                var style_68768a = new Theme68768a();
                if (!application.Resources.MergedDictionaries.Contains(style_68768a))
                    application.Resources.MergedDictionaries.Add(style_68768a);
                break;
            case "#515c6b":
                var style_515c6b = new Theme515c6b();
                if (!application.Resources.MergedDictionaries.Contains(style_515c6b))
                    application.Resources.MergedDictionaries.Add(style_515c6b);
                break;
            case "#567c73":
                var style_567c73 = new Theme567c73();
                if (!application.Resources.MergedDictionaries.Contains(style_567c73))
                    application.Resources.MergedDictionaries.Add(style_567c73);
                break;
            case "#486860":
                var style_486860 = new Theme486860();
                if (!application.Resources.MergedDictionaries.Contains(style_486860))
                    application.Resources.MergedDictionaries.Add(style_486860);
                break;
            case "#498205":
                var style_498205 = new Theme498205();
                if (!application.Resources.MergedDictionaries.Contains(style_498205))
                    application.Resources.MergedDictionaries.Add(style_498205);
                break;
            case "#107c10":
                var style_107c10 = new Theme107c10();
                if (!application.Resources.MergedDictionaries.Contains(style_107c10))
                    application.Resources.MergedDictionaries.Add(style_107c10);
                break;
            case "#767676":
                var style_767676 = new Theme767676();
                if (!application.Resources.MergedDictionaries.Contains(style_767676))
                    application.Resources.MergedDictionaries.Add(style_767676);
                break;
            case "#4c4a48":
                var style_4c4a48 = new Theme4c4a48();
                if (!application.Resources.MergedDictionaries.Contains(style_4c4a48))
                    application.Resources.MergedDictionaries.Add(style_4c4a48);
                break;
            case "#69797e":
                var style_69797e = new Theme69797e();
                if (!application.Resources.MergedDictionaries.Contains(style_69797e))
                    application.Resources.MergedDictionaries.Add(style_69797e);
                break;
            case "#4a5459":
                var style_4a5459 = new Theme4a5459();
                if (!application.Resources.MergedDictionaries.Contains(style_4a5459))
                    application.Resources.MergedDictionaries.Add(style_4a5459);
                break;
            case "#647c64":
                var style_647c64 = new Theme647c64();
                if (!application.Resources.MergedDictionaries.Contains(style_647c64))
                    application.Resources.MergedDictionaries.Add(style_647c64);
                break;
            case "#525e54":
                var style_525e54 = new Theme525e54();
                if (!application.Resources.MergedDictionaries.Contains(style_525e54))
                    application.Resources.MergedDictionaries.Add(style_525e54);
                break;
            case "#847545":
                var style_847545 = new Theme847545();
                if (!application.Resources.MergedDictionaries.Contains(style_847545))
                    application.Resources.MergedDictionaries.Add(style_847545);
                break;
            case "#7e735f":
                var style_7e735f = new Theme7e735f();
                if (!application.Resources.MergedDictionaries.Contains(style_7e735f))
                    application.Resources.MergedDictionaries.Add(style_7e735f);
                break;
            case "#ffb900":
            default:
                var style_ffb900 = new Themeffb900();
                if (!application.Resources.MergedDictionaries.Contains(style_ffb900))
                    application.Resources.MergedDictionaries.Add(style_ffb900);
                break;
        }

        return application;
    }
}
