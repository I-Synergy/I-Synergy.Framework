This code is extending the basic functions of the built in MAUI Essentials IPreferences by letting the developers to save any type of object.
This is done by using JSON serialization, and saving the raw jsons as strings with the default IPreferences implementation.
Please consider saving large objects with it.
Since JSON arrays can be serialized back to any collection type, you can use different types when Setting the value and when Getting it back.

### Method 1 - Dependency injection

If you used to dependency inject the IPreferences class, you can use this tool without any differencies. Call the SetObject or GetObject extension method on the IPreferences.

```csharp
    private readonly IPreferences _preferences;

    public MainPage(IPreferences preferences)
    {
        InitializeComponent();
        _preferences = preferences;
    }

    private void Foo()
    {
        List<string> taleItems = new List<string>()
        {
            "The quick brown fox",
            "Jumps over the lazy dog"
        };

        _preferences.SetObject<List<string>>("Tale", taleItems);

        string[] taleItemsFromPreferences = _preferences.GetObject<string[]>("Tale", null);
    }
```

### Method 2 - Static preferences

If you are used to access the preferences trough the static class, you can use this tool by accessing the Default property on the Preferences class. You can call the SetObject or GetObject extension method on it.

```csharp
    // Setting the value
    Preferences.Default.SetObject<IDeviceInfo>("Device_Information", DeviceInfo.Current);
    
    // Getting the value
    Preferences.Default.GetObject<IDeviceInfo>("Device_Information", null);
```