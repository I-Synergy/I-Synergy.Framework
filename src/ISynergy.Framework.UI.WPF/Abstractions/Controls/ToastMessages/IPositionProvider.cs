using ISynergy.Framework.UI.Controls.ToastNotification.Enumerations;
using System.Windows;

namespace ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;

public interface IPositionProvider : IDisposable
{
    System.Windows.Window ParentWindow { get; }
    Point GetPosition(double actualPopupWidth, double actualPopupHeight);
    double GetHeight();
    EjectDirection EjectDirection { get; }

    event EventHandler UpdatePositionRequested;
    event EventHandler UpdateEjectDirectionRequested;
    event EventHandler UpdateHeightRequested;
}
