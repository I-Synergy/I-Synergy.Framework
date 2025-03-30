using System.ComponentModel;

namespace ISynergy.Framework.Core.Tests.Data.TestClasses;

public class Alarm
{
    public event PropertyChangedEventHandler? Beeped;

    public void Beep()
    {
        Beeped?.Invoke(this, new PropertyChangedEventArgs("Beep!"));
    }
}
