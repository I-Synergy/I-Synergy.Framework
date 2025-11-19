# AsyncRelayCommand and RelayCommand - CommandParameter Usage Guide

## Overview
This document explains how `CommandParameter` works in the ISynergy.Framework MVVM commands and provides best practices for proper usage.

## Command Types

### 1. **Non-Generic Commands** (`AsyncRelayCommand`, `RelayCommand`)
These commands **DO NOT** use the `CommandParameter`:

```csharp
// ? Parameter is IGNORED
var command = new AsyncRelayCommand(async () => 
{
    // No parameter available here
    await DoSomethingAsync();
});

// Even if you bind CommandParameter in XAML, it's ignored:
<Button Command="{Binding MyCommand}" CommandParameter="{Binding SelectedItem}" />
```

**Use case**: Simple commands that don't need external data.

---

### 2. **Generic Commands** (`AsyncRelayCommand<T>`, `RelayCommand<T>`)
These commands **DO** use the `CommandParameter`:

```csharp
// ? Parameter is PASSED
var command = new AsyncRelayCommand<string>(async (param) => 
{
    // param contains the CommandParameter value
    await ProcessAsync(param);
});

// XAML binding works correctly:
<Button Command="{Binding MyCommand}" CommandParameter="{Binding SelectedItem}" />
```

**Use case**: Commands that need to operate on specific data passed from the UI.

---

## CommandParameter with ViewModel Properties

### ? **Problem**: ThemeWindow Scenario (Before Fix)

```csharp
// ViewModel
public class ThemeViewModel : ViewModelDialog<ThemeStyle>
{
  public ThemeViewModel(...)
    {
        // OLD CODE - Checks parameter only
        SubmitCommand = new AsyncRelayCommand<ThemeStyle>(
        execute: async e => await SubmitAsync(e), 
     canExecute: e => e is not null);  // ? MAUI doesn't re-evaluate visual state correctly
    }
}
```

**Issue**: When `SelectedItem` changes, `CanExecute` doesn't re-evaluate properly in MAUI because it only checks the parameter, not the property.

---

### ? **Solution**: Check Both Parameter AND Property

```csharp
// ViewModel
public class ThemeViewModel : ViewModelDialog<ThemeStyle>
{
    public ThemeViewModel(...)
    {
        // NEW CODE - Checks parameter OR property
        SubmitCommand = new AsyncRelayCommand<ThemeStyle>(
          execute: async e => await SubmitAsync(e ?? SelectedItem!), 
         canExecute: e => (e ?? SelectedItem) is not null);  // ? Works correctly in MAUI
    }
}
```

**How it works**:
1. `CanExecute` checks **parameter first**, then falls back to `SelectedItem` property
2. When `SelectedItem` changes ? `NotifyCanExecuteChanged()` is called
3. MAUI re-evaluates `CanExecute`, which accesses the `SelectedItem` property
4. Button visual state updates correctly

---

## Best Practices

### ? **Recommended Pattern for ViewModel Commands**

```csharp
public class MyViewModel : ViewModelDialog<MyEntity>
{
    public AsyncRelayCommand<MyEntity> SubmitCommand { get; private set; }

    public MyViewModel(...)
    {
        SubmitCommand = new AsyncRelayCommand<MyEntity>(
            execute: async e => await SubmitAsync(e ?? SelectedItem!), 
 canExecute: e => (e ?? SelectedItem) is not null);
    }

    public async Task SubmitAsync(MyEntity entity)
    {
        // Use the entity
        await SaveAsync(entity);
    }
}
```

**Benefits**:
- ? Works when parameter is passed from XAML binding
- ? Works when called programmatically without parameter
- ? MAUI Button visual state updates correctly
- ? Property changes trigger command re-evaluation

---

### ? **XAML Binding**

```xaml
<!-- Method 1: Bind CommandParameter (recommended for dialogs) -->
<Button 
    Command="{Binding SubmitCommand}" 
    CommandParameter="{Binding SelectedItem}" 
    Text="OK" />

<!-- Method 2: No CommandParameter (fallback to property) -->
<Button 
    Command="{Binding SubmitCommand}" 
    Text="OK" />
```

Both work correctly with the recommended pattern!

---

## Common Scenarios

### 1. **Dialog Commands** (ThemeWindow, LanguageWindow, etc.)

```csharp
// ViewModel
public class ThemeViewModel : ViewModelDialog<ThemeStyle>
{
    public AsyncRelayCommand<ThemeStyle> SubmitCommand { get; private set; }

    public ThemeViewModel(...)
    {
        SubmitCommand = new AsyncRelayCommand<ThemeStyle>(
  execute: async e => await SubmitAsync(e ?? SelectedItem!), 
    canExecute: e => (e ?? SelectedItem) is not null);
    }
}
```

```xaml
<!-- XAML -->
<Button Command="{Binding SubmitCommand}" CommandParameter="{Binding SelectedItem}" Text="OK" />
```

---

### 2. **List Commands** (Edit, Delete, etc.)

```csharp
// ViewModel
public class CustomersViewModel : ViewModelBladeView<Customer>
{
    public AsyncRelayCommand<Customer> EditCommand { get; private set; }

    public CustomersViewModel(...)
    {
        EditCommand = new AsyncRelayCommand<Customer>(
   execute: async customer => await EditAsync(customer),
         canExecute: customer => customer is not null);
    }
}
```

```xaml
<!-- XAML -->
<ListView ItemsSource="{Binding Items}">
    <ListView.ItemTemplate>
        <DataTemplate>
            <ViewCell>
         <Button 
          Command="{Binding Source={RelativeSource AncestorType={x:Type local:CustomersViewModel}}, Path=EditCommand}" 
                    CommandParameter="{Binding .}" 
   Text="Edit" />
            </ViewCell>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

---

### 3. **Simple Commands** (No Parameter Needed)

```csharp
// ViewModel
public class SettingsViewModel : ViewModel
{
    public AsyncRelayCommand SaveCommand { get; private set; }

    public SettingsViewModel(...)
    {
        // Non-generic command - doesn't need parameter
        SaveCommand = new AsyncRelayCommand(async () => await SaveSettingsAsync());
    }
}
```

```xaml
<!-- XAML -->
<Button Command="{Binding SaveCommand}" Text="Save" />
```

---

## Test Coverage

The following scenarios are covered by unit tests:

| Test | Description |
|------|-------------|
| `GenericCommand_WithParameter_PassesParameterCorrectly` | Verifies parameter is passed to execute method |
| `GenericCommand_CanExecute_UsesParameterInPredicate` | Verifies CanExecute checks parameter |
| `GenericCommand_WithNullParameter_HandlesGracefully` | Verifies null parameters are handled |
| `GenericCommand_WithFallbackToProperty_UsesParameterFirst` | Verifies fallback pattern (parameter first) |
| `GenericCommand_WithFallbackToProperty_UsesFallbackWhenParameterNull` | Verifies fallback pattern (property second) |
| `GenericCommand_CanExecute_ChecksPropertyWhenParameterNull` | Verifies CanExecute fallback behavior |
| `NonGenericCommand_IgnoresParameter` | Verifies non-generic commands ignore parameters |

Run tests:
```bash
dotnet test --filter "FullyQualifiedName~CommandParameter"
```

---

## Summary

| Command Type | Uses CommandParameter? | Example |
|--------------|------------------------|---------|
| `AsyncRelayCommand` | ? No | `new AsyncRelayCommand(async () => {...})` |
| `AsyncRelayCommand<T>` | ? Yes | `new AsyncRelayCommand<T>(async (param) => {...})` |
| `RelayCommand` | ? No | `new RelayCommand(() => {...})` |
| `RelayCommand<T>` | ? Yes | `new RelayCommand<T>((param) => {...})` |

**Key Takeaway**: Always use the **generic version** (`AsyncRelayCommand<T>`) when you need to use `CommandParameter` in XAML bindings!

**MAUI Fix**: Use the fallback pattern `e => (e ?? SelectedItem) is not null` to ensure Button visual state updates correctly when properties change.
