using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Extensions;

namespace ISynergy.Framework.UI.Behaviors;
public class CursorBehavior
{
    public static readonly BindableProperty CursorProperty = BindableProperty.CreateAttached("Cursor", typeof(CursorIcons), typeof(CursorBehavior), CursorIcons.Arrow, propertyChanged: CursorChanged);

    private static void CursorChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is VisualElement visualElement)
        {
            visualElement.SetCustomCursor((CursorIcons)newvalue, visualElement.Handler?.MauiContext ?? Application.Current?.Windows.LastOrDefault()?.Page?.Handler?.MauiContext);
        }
    }

    public static CursorIcons GetCursor(BindableObject view) => (CursorIcons)view.GetValue(CursorProperty);

    public static void SetCursor(BindableObject view, CursorIcons value) => view.SetValue(CursorProperty, value);
}