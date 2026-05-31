using System;
using System.Reflection;
using Microsoft.Windows.AppLifecycle;

class Program {
    static void Main() {
        var t = typeof(AppActivationArguments);
        Console.WriteLine($"Type: {t.FullName}");
        Console.WriteLine($"IsPublic: {t.IsPublic}");
        Console.WriteLine($"IsValueType: {t.IsValueType}");
        Console.WriteLine("Constructors:");
        foreach (var ctor in t.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
            Console.Write($"  {ctor} -> params: ");
            foreach (var p in ctor.GetParameters()) {
                Console.Write($"{p.ParameterType.Name} {p.Name}, ");
            }
            Console.WriteLine();
        }
        Console.WriteLine("Properties:");
        foreach (var prop in t.GetProperties()) {
            var setter = prop.CanWrite ? "(w)" : "(r)";
            Console.WriteLine($"  {prop.Name} {setter}: {prop.PropertyType.Name}");
        }
        // Try creating
        try {
            var instance = Activator.CreateInstance(t, nonPublic: true);
            Console.WriteLine($"Created via nonPublic: {instance}");
            Console.WriteLine($"Kind: {t.GetProperty("Kind")?.GetValue(instance)}");
        } catch (Exception ex) {
            Console.WriteLine($"nonPublic failed: {ex.Message}");
        }
        try {
            var instance = Activator.CreateInstance(t);
            Console.WriteLine($"Created via default: {instance}");
        } catch (Exception ex) {
            Console.WriteLine($"Default failed: {ex.Message}");
        }
        try {
            var instance = Activator.CreateInstance(t, new object[] { ExtendedActivationKind.Launch });
            Console.WriteLine($"Created via (kind): {instance}");
        } catch (Exception ex) {
            Console.WriteLine($"(kind) failed: {ex.Message}");
        }
    }
}
