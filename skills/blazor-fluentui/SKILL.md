```skill
{
  "name": "i-synergy-blazor-ui",
  "description": "Production-ready Blazor patterns exactly as implemented inside the I-Synergy Framework UI (View<TViewModel>, ConfigureServices bootstrapper, CommonServices, Dialog/Navigation services).",
  "version": "2026.02.10",
  "sources": [
    "src/ISynergy.Framework.UI.Blazor",
    "src/ISynergy.Framework.Mvvm",
    "samples/Sample.Blazor",
    "samples/Sample.Maui",
    "samples/Sample.WinUI",
    "samples/Sample.WPF"
  ],
  "license": "See license.md in repository root"
}
```

# I-Synergy Framework Blazor Skill (SKILL.md)

**Version**: 2026.02.10
**Target**: I-Synergy Framework UI stack on Blazor Server / Interactive
**UI Library**: Microsoft Fluent UI Blazor Components + ISynergy controls
**Purpose**: Document the concrete MVVM-first patterns used across the official WPF/WinUI/MAUI/Blazor samples so other projects can reproduce the exact architecture.

This skill is sourced directly from the production packages in [src/ISynergy.Framework.UI.Blazor](src/ISynergy.Framework.UI.Blazor/readme.md) and the runnable sample apps under [samples](samples). The remaining sections keep the quick-reference tables for Fluent UI and Blazor fundamentals, but the first part below explains the I-Synergy-specific conventions that must be followed before falling back to generic guidance.

---

## I-Synergy Architecture Highlights

- **Single bootstrapper**: All apps call `builder.Services.ConfigureServices<Context, CommonServices, ExceptionHandlerService, SettingsService, Resources>(...)` to register MVVM services, scoped contexts, validation, and localization. See [samples/Sample.Blazor/Program.cs](samples/Sample.Blazor/Program.cs#L1-L62).
- **MVVM-first Razor components**: UI files inherit `View<TViewModel>` from [src/ISynergy.Framework.UI.Blazor/Components/Controls/View.cs](src/ISynergy.Framework.UI.Blazor/Components/Controls/View.cs#L13-L111) so Dependency Injection provides the ViewModel, initializes it once, subscribes to `PropertyChanged`, and wires every `ICommand` automatically.
- **Command binding helpers**: Use `@this.CommandBinding(ViewModel.SomeCommand)` from [src/ISynergy.Framework.UI.Blazor/Extensions/ComponentExtensions.cs](src/ISynergy.Framework.UI.Blazor/Extensions/ComponentExtensions.cs#L5-L33) to get button click handlers plus disabled state that respects `CanExecute`.
- **CommonServices hub**: ViewModels inherit from [src/ISynergy.Framework.Mvvm/ViewModels](src/ISynergy.Framework.Mvvm/readme.md) and receive `ICommonServices`, giving access to `BusyService`, `DialogService`, `NavigationService`, `LanguageService`, and a scoped child container for spawning dialog ViewModels.
- **Fluent UI opinionated defaults**: All layouts use `FluentStack`, `FluentCard`, and Microsoft Fluent icons to match the WinUI / MAUI theme system. The same colors flow from the Theme service so accent changes stay consistent cross-platform.

### Repository Assets Worth Skimming

| Asset | Why it matters |
| --- | --- |
| [samples/Sample.Blazor](samples/Sample.Blazor) | Canonical server-side app showing command binding, dialogs, exception handling, and Fluent UI layouts. |
| [samples/Sample.Maui](samples/Sample.Maui) | Shares the same ViewModels while hosting them in MAUI XAML views (`c:View`). Demonstrates theme switching and file services. |
| [samples/Sample.WPF](samples/Sample.WPF) & [samples/Sample.WinUI](samples/Sample.WinUI) | Prove that the same `ViewModel` hierarchy is reused across desktop shells. |
| [src/ISynergy.Framework.UI.Blazor/readme.md](src/ISynergy.Framework.UI.Blazor/readme.md) | Detailed feature list for the Blazor package (form factor service, antiforgery integration, HTTP resilience). |

### Bootstrapping (exact code used in Sample.Blazor)

```csharp
var mainAssembly = Assembly.GetExecutingAssembly();
var infoService = new InfoService();
infoService.LoadAssembly(mainAssembly);

builder.Services
    .ConfigureServices<Context, CommonServices, ExceptionHandlerService, SettingsService, Properties.Resources>(
        builder.Configuration,
        infoService,
        services =>
        {
            builder.Services.AddSingleton(_ => DefaultJsonSerializers.Web);
            builder.Services.AddRazorComponents().AddInteractiveServerComponents();
            builder.Services.AddFluentUIComponents();
            builder.Services.AddSingleton<INavigationMenuService, NavigationMenuService>();
        },
        mainAssembly,
        f => f.Name!.StartsWith(typeof(Program).Namespace!));

var app = builder
    .Build()
    .SetLocatorProvider();
```

> Key takeaways: the `ConfigureServices` extension wires the MVVM runtime, registers `Context`, and limits auto-scan to assemblies that belong to the current app. `SetLocatorProvider()` exposes the service provider to the `ScopedContextService`, letting dialogs resolve child ViewModels on demand.

### View + ViewModel pairing

Razor components inherit the `View<TViewModel>` control so every binding goes through the shared ViewModel:

```razor
@page "/commanddemoview"
@using ISynergy.Framework.AspNetCore.Blazor.Components.Controls
@using ISynergy.Framework.UI.Extensions
@rendermode InteractiveServer
@inherits View<CommandDemoViewModel>

<h3>Command Binding Demo</h3>

<button @attributes="@this.CommandBinding(ViewModel.DecrementCommand)">
    Decrement
</button>

<button @attributes="@this.CommandBinding(ViewModel.OpenSampleDialogCommand)">
    Open Sample Dialog
</button>
```

See [samples/Sample.Blazor/Components/Pages/CommandDemoView.razor](samples/Sample.Blazor/Components/Pages/CommandDemoView.razor). The helper automatically disables buttons when `CanExecute` returns `false`.

Each ViewModel inherits from `ViewModel`, `ViewModelDialog<T>`, or `ViewModelNavigation<T>` so it has access to `CommonServices`:

```csharp
public class CommandDemoViewModel : ViewModel
{
    private readonly IDialogService _dialogService;

    public int Counter
    {
        get => GetValue<int>();
        set => SetValue(value);
    }

    public RelayCommand IncrementCommand { get; }
    public AsyncRelayCommand OpenSampleDialogCommand { get; }

    public CommandDemoViewModel(ICommonServices commonServices,
        IDialogService dialogService,
        ILogger<CommandDemoViewModel> logger)
        : base(commonServices, logger)
    {
        _dialogService = dialogService;
        IncrementCommand = new RelayCommand(() => Counter++);
        OpenSampleDialogCommand = new AsyncRelayCommand(OpenSampleDialogAsync);
    }
}
```

Referenced in [samples/Sample.Blazor/ViewModels/CommandDemoViewModel.cs](samples/Sample.Blazor/ViewModels/CommandDemoViewModel.cs#L1-L78). Dialogs are just Razor components bound to `ViewModelDialog<T>` (see [samples/Sample.Blazor/ViewModels/SampleDialogViewModel.cs](samples/Sample.Blazor/ViewModels/SampleDialogViewModel.cs)).

### Cross-platform reuse

The same ViewModel hierarchy powers MAUI (see [samples/Sample.Maui/ViewModels](samples/Sample.Maui/ViewModels)) and desktop shells. This is why all templates in this skill now assume a `View<TViewModel>` wrapper and `CommandBinding`. Keep ViewModels UI-agnostic by sticking to `SetValue/ GetValue`, `CommonServices.DialogService`, `BusyService`, and `NavigationService`.

---

## Blazor Expert Resource Pack (merged content)

The original **blazor-expert** skill shipped six deep reference files that still apply to every ISynergy UI. They now live under [resources](resources) inside this consolidated skill so you can pull in generic Blazor know-how when needed.

| Task | Load Resource | Key Topics |
|------|---------------|-----------|
| **Build components, handle lifecycle events** | [components-lifecycle.md](resources/components-lifecycle.md) | Component structure, lifecycle methods, parameters, cascading values, RenderFragment composition |
| **Manage component state, handle events** | [state-management-events.md](resources/state-management-events.md) | Local state, EventCallback, data binding, cascading state, service-based state |
| **Configure routes, navigate between pages** | [routing-navigation.md](resources/routing-navigation.md) | Route parameters, constraints, navigation, NavLink, query strings, layouts |
| **Build forms, validate user input** | [forms-validation.md](resources/forms-validation.md) | EditForm, input components, DataAnnotations validation, custom validators |
| **Setup authentication & authorization** | [authentication-authorization.md](resources/authentication-authorization.md) | Auth setup, AuthorizeView, Authorize attribute, policies, claims |
| **Optimize performance, use JavaScript interop** | [performance-advanced.md](resources/performance-advanced.md) | Rendering optimization, virtualization, JS interop, lazy loading, WASM best practices |

**How to use the pack**

1. **Task analysis** – map the feature you are building to the table above.
2. **Load the file** – open the relevant resource and search for specific sub-topics.
3. **Implement + validate** – adapt the patterns into the ISynergy MVVM architecture, then test in Server/Interactive modes.

### Pre-defined workflows

- **Data entry experience**: components-lifecycle → state-management-events → forms-validation.
- **Auth + protected pages**: authentication-authorization → routing-navigation.
- **Searchable list**: routing-navigation (query strings) → state-management-events (binding) → performance-advanced (virtualization).
- **Perf audit**: performance-advanced end-to-end (ShouldRender, @key, JS module isolation).

### Key concept quick hits

- **Component architecture**: parameters, cascading values, RenderFragment composition.
- **State management**: prioritize immutable updates and scoped services for shared state.
- **Routing**: use constraints (`:guid`, `:int`) and `NavLink` for active styling.
- **Forms**: pair `EditForm` with `DataAnnotationsValidator`, surface errors via `ValidationMessage`.
- **Auth**: policies over roles; always validate on the server; AuthorizeView for differential UI.
- **Performance**: `@key`, `ShouldRender`, virtualization, JS interop modules, AOT for WASM when applicable.

These summaries keep the frequently referenced knowledge from **blazor-expert** alive without forcing you to juggle another skill card.

---

## Design Direction Playbook (from frontend-design)

To replace the standalone **frontend-design** skill, this section captures the critical aesthetic rules we follow when crafting Fluent UI + ISynergy experiences:

- **Pick a bold aesthetic**: define purpose, audience, and a memorable tone (brutalist, retro-futuristic, soft industrial, etc.) before writing markup.
- **Typography**: avoid system defaults (Inter/Roboto/Arial). Pair a distinctive display face with a refined body font and wire both through CSS variables so they propagate to Fluent tokens.
- **Color systems**: declare semantic CSS variables (e.g., `--surface`, `--ink`, `--accent`) and stick to high-contrast palettes. Commit to either dramatic gradients/patterns or razor-sharp minimalism—never bland middle ground.
- **Background + atmosphere**: build depth with layered gradients, subtle noise, glassmorphism, or geometric patterns that align with the chosen tone.
- **Motion**: prefer a few intentional animations (page-load stagger, command palette reveal) instead of scattered micro-jiggles. Use CSS keyframes or Fluent `Motion` helpers where available.
- **Spatial composition**: embrace asymmetry, overlapping cards, and meaningful negative space. Layout choices should reinforce the story of the screen.
- **Accessibility**: maintain WCAG contrast, provide focus states, respect reduced-motion preferences, and keep keyboard traps out of dialog UX.

Whenever you generate markup in this skill, bake in those decisions (fonts, variables, animations) so downstream apps inherit a premium look instead of generic AI scaffolding.

---

## 1. Blazor Hosting Models

### Blazor Server

**Characteristics**:
- Server-side rendering with SignalR connection
- Stateful connection to server required
- Small initial download (~2MB), fast startup
- UI events sent to server via SignalR

**Setup**:
```csharp
// Program.cs
builder.Services.AddServerSideBlazor();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
```

**Use Cases**: Internal apps, intranet, enterprise applications

---

### Blazor WebAssembly

**Characteristics**:
- Client-side execution in browser via WebAssembly
- Full .NET runtime in browser
- Larger initial download (~10MB), but offline capable
- No server connection after initial load

**Setup**:
```csharp
// Program.cs (Client)
builder.Services.AddBlazorWebAssembly();
await builder.Build().RunAsync();
```

**Use Cases**: Public-facing apps, PWAs, offline-first applications

---

### Render Modes (.NET 10+)

```razor
@* Server-side with SignalR *@
@rendermode InteractiveServer

@* Client-side WebAssembly *@
@rendermode InteractiveWebAssembly

@* Auto: Server initially, then WebAssembly after download *@
@rendermode InteractiveAuto

@* Static: Server-side rendering without interactivity *@
@rendermode @(new Microsoft.AspNetCore.Components.Web.RenderMode.Static)
```

---

## 2. Component Architecture

### Basic Component Structure

```razor
@page "/counter"
@using MyApp.Services

<h3>@Title</h3>
<p>Current count: @currentCount</p>
<button @onclick="IncrementCount">Click me</button>

@code {
    [Parameter]
    public string Title { get; set; } = "Counter";

    [Parameter]
    public EventCallback<int> OnCountChanged { get; set; }

    private int currentCount = 0;

    private async Task IncrementCount()
    {
        currentCount++;
        await OnCountChanged.InvokeAsync(currentCount);
    }
}
```

---

### Component Parameters

```csharp
// Required parameter
[Parameter, EditorRequired]
public string Name { get; set; } = "";

// Optional parameter with default
[Parameter]
public int MaxCount { get; set; } = 10;

// EventCallback (parent-child communication)
[Parameter]
public EventCallback<string> OnValueChanged { get; set; }

// Cascading parameter (from ancestor)
[CascadingParameter]
public AppState AppState { get; set; } = default!;

// Capture unmatched attributes
[Parameter(CaptureUnmatchedValues = true)]
public Dictionary<string, object>? AdditionalAttributes { get; set; }
```

---

### Component Lifecycle

```csharp
// 1. Parameters set (before initialization)
public override void SetParametersAsync(ParameterView parameters)
{
    base.SetParametersAsync(parameters);
}

// 2. Component initialization (once)
protected override void OnInitialized()
{
    // Synchronous initialization
}

protected override async Task OnInitializedAsync()
{
    // Async initialization (preferred for data loading)
    data = await DataService.GetDataAsync();
}

// 3. Parameters set (every time parameters change)
protected override void OnParametersSet()
{
    // React to parameter changes
}

protected override async Task OnParametersSetAsync()
{
    // Async parameter processing
}

// 4. After rendering
protected override void OnAfterRender(bool firstRender)
{
    if (firstRender)
    {
        // One-time setup after first render
    }
}

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        // JS interop, focus management
        await JS.InvokeVoidAsync("initializeComponent");
    }
}

// 5. Determine if re-render needed
protected override bool ShouldRender()
{
    // Return false to skip re-render
    return true;
}

// 6. Disposal (cleanup)
public void Dispose()
{
    // Unsubscribe from events, dispose resources
    AppState.OnChange -= StateHasChanged;
}

public async ValueTask DisposeAsync()
{
    // Async disposal
    if (hubConnection is not null)
    {
        await hubConnection.DisposeAsync();
    }
}
```

---

## 3. Fluent UI Components

### Layout Components

```razor
@using Microsoft.FluentUI.AspNetCore.Components

@* Vertical stack with gap *@
<FluentStack Orientation="Orientation.Vertical" VerticalGap="16">
    <h1>Title</h1>
    <p>Content</p>
</FluentStack>

@* Horizontal stack *@
<FluentStack Orientation="Orientation.Horizontal" HorizontalGap="8">
    <FluentButton>Cancel</FluentButton>
    <FluentButton Appearance="Appearance.Accent">Save</FluentButton>
</FluentStack>

@* Grid layout *@
<FluentGrid Spacing="16">
    <FluentGridItem xs="12" sm="6" md="4">
        <FluentCard>Column 1</FluentCard>
    </FluentGridItem>
    <FluentGridItem xs="12" sm="6" md="4">
        <FluentCard>Column 2</FluentCard>
    </FluentGridItem>
</FluentGrid>
```

---

### Input Components

```razor
@* Text input *@
<FluentTextField
    @bind-Value="name"
    Label="Name"
    Placeholder="Enter your name"
    Required="true" />

@* Text area *@
<FluentTextArea
    @bind-Value="description"
    Label="Description"
    Rows="4" />

@* Number input *@
<FluentNumberField
    @bind-Value="age"
    Label="Age"
    Min="0"
    Max="150" />

@* Select dropdown *@
<FluentSelect
    @bind-Value="selectedOption"
    Label="Choose option"
    Items="@options"
    OptionText="@(x => x.Name)"
    OptionValue="@(x => x.Id)" />

@* Checkbox *@
<FluentCheckbox
    @bind-Value="accepted"
    Label="I accept the terms" />

@* Switch toggle *@
<FluentSwitch
    @bind-Value="isEnabled"
    Label="Enable feature" />

@* Date picker *@
<FluentDatePicker
    @bind-Value="selectedDate"
    Label="Select date" />
```

---

### Buttons

```razor
@* Primary button *@
<FluentButton Appearance="Appearance.Accent" OnClick="Submit">
    Submit
</FluentButton>

@* Secondary button *@
<FluentButton Appearance="Appearance.Neutral">
    Cancel
</FluentButton>

@* Icon button *@
<FluentIconButton
    Icon="@(new Icons.Regular.Size20.Add())"
    aria-label="Add item"
    OnClick="AddItem" />

@* Split button *@
<FluentSplitButton Text="Actions">
    <FluentMenuItem OnClick="Edit">Edit</FluentMenuItem>
    <FluentMenuItem OnClick="Delete">Delete</FluentMenuItem>
</FluentSplitButton>
```

---

### Data Display

```razor
@* Card *@
<FluentCard>
    <h3>Card Title</h3>
    <p>Card content goes here.</p>
</FluentCard>

@* Data grid with sorting *@
<FluentDataGrid Items="@users" Virtualize="true">
    <PropertyColumn Property="@(u => u.Name)" Sortable="true" />
    <PropertyColumn Property="@(u => u.Email)" />
    <PropertyColumn Property="@(u => u.CreatedAt)" Format="yyyy-MM-dd" Sortable="true" />
    <TemplateColumn Title="Actions">
        <FluentButton OnClick="@(() => EditUser(context))">Edit</FluentButton>
    </TemplateColumn>
</FluentDataGrid>

@* Accordion *@
<FluentAccordion>
    <FluentAccordionItem Heading="Section 1">
        Content for section 1
    </FluentAccordionItem>
    <FluentAccordionItem Heading="Section 2">
        Content for section 2
    </FluentAccordionItem>
</FluentAccordion>
```

---

### Feedback Components

```razor
@* Dialog *@
<FluentDialog @bind-Hidden="hideDialog" Modal="true">
    <h2>Confirmation</h2>
    <p>Are you sure you want to delete this item?</p>
    <FluentButton Appearance="Appearance.Accent" OnClick="ConfirmDelete">
        Yes, Delete
    </FluentButton>
    <FluentButton OnClick="@(() => hideDialog = true)">
        Cancel
    </FluentButton>
</FluentDialog>

@* Message bar (notification) *@
<FluentMessageBar Intent="MessageIntent.Success" Visible="@showSuccess">
    Item saved successfully!
</FluentMessageBar>

@* Progress ring (loading spinner) *@
<FluentProgressRing Visible="@isLoading" />

@* Toast notification *@
@inject IToastService ToastService

@code {
    private void ShowToast()
    {
        ToastService.ShowSuccess("Operation completed successfully!");
    }
}
```

---

## 4. State Management

### Component State

```csharp
@code {
    // Private field for component-local state
    private int count = 0;
    private string message = "";
    private List<Item> items = new();

    // State change triggers re-render automatically
    private void UpdateState()
    {
        count++;
        // Component re-renders automatically
    }
}
```

---

### Service-Based State

```csharp
// AppState.cs
public class AppState
{
    private string currentUser = "";

    public string CurrentUser
    {
        get => currentUser;
        set
        {
            if (currentUser != value)
            {
                currentUser = value;
                NotifyStateChanged();
            }
        }
    }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}

// Program.cs
builder.Services.AddScoped<AppState>();

// Component
@inject AppState AppState
@implements IDisposable

@code {
    protected override void OnInitialized()
    {
        AppState.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        AppState.OnChange -= StateHasChanged;
    }

    private void UpdateUser()
    {
        AppState.CurrentUser = "John Doe";
        // All subscribed components re-render
    }
}
```

---

### Cascading Values

```razor
@* App.razor or parent component *@
<CascadingValue Value="@currentTheme">
    <Router AppAssembly="@typeof(Program).Assembly">
        @* Child components *@
    </Router>
</CascadingValue>

@code {
    private string currentTheme = "light";
}

@* Child component *@
@code {
    [CascadingParameter]
    public string Theme { get; set; } = "";

    // Access Theme without prop drilling
}
```

---

## 5. Forms & Validation

### Basic EditForm

```razor
<EditForm Model="@person" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <FluentTextField @bind-Value="person.Name" Label="Name" />
    <ValidationMessage For="@(() => person.Name)" />

    <FluentNumberField @bind-Value="person.Age" Label="Age" />
    <ValidationMessage For="@(() => person.Age)" />

    <FluentButton Type="ButtonType.Submit" Appearance="Appearance.Accent">
        Submit
    </FluentButton>
</EditForm>

@code {
    private Person person = new();

    private async Task HandleValidSubmit()
    {
        // Form is valid
        await SavePersonAsync(person);
    }

    public class Person
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [Range(0, 150, ErrorMessage = "Age must be between 0 and 150")]
        public int Age { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}
```

---

### FluentValidation

```csharp
// Install: FluentValidation.Blazor
using FluentValidation;

public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100);

        RuleFor(x => x.Age)
            .InclusiveBetween(0, 150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}

// Component
<EditForm Model="@person" OnValidSubmit="HandleValidSubmit">
    <FluentValidationValidator />
    <ValidationSummary />
    @* Form fields *@
</EditForm>
```

---

### Custom Validation

```csharp
// Custom attribute
public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime date && date > DateTime.Now)
        {
            return ValidationResult.Success;
        }
        return new ValidationResult("Date must be in the future");
    }
}

// Usage
public class Event
{
    [FutureDate]
    public DateTime EventDate { get; set; }
}
```

---

## 6. Routing & Navigation

### Route Declaration

```razor
@* Basic route *@
@page "/products"

@* Multiple routes *@
@page "/products"
@page "/items"

@* Route parameter *@
@page "/product/{id}"

@code {
    [Parameter]
    public string Id { get; set; } = "";
}

@* Optional parameter *@
@page "/product/{id?}"

@* Route constraint *@
@page "/product/{id:int}"
@page "/user/{userId:guid}"

@* Catch-all parameter *@
@page "/docs/{*path}"
```

---

### Navigation

```csharp
@inject NavigationManager Navigation

@code {
    private void NavigateToProduct(int id)
    {
        Navigation.NavigateTo($"/product/{id}");
    }

    private void NavigateExternal()
    {
        Navigation.NavigateTo("https://example.com", forceLoad: true);
    }

    private void Refresh()
    {
        Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
    }

    // Get current URL
    var currentUrl = Navigation.Uri;
    var baseUrl = Navigation.BaseUri;

    // Query string
    var uri = new Uri(Navigation.Uri);
    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
    var searchTerm = query["search"];
}
```

---

### NavLink Component

```razor
<FluentNavMenu>
    @* Exact match (active only for exact path) *@
    <FluentNavLink Href="/" Match="NavLinkMatch.All">
        Home
    </FluentNavLink>

    @* Prefix match (active for path and descendants) *@
    <FluentNavLink Href="/products" Match="NavLinkMatch.Prefix">
        Products
    </FluentNavLink>
</FluentNavMenu>
```

---

## 7. JavaScript Interop

### Calling JavaScript from Blazor

```csharp
@inject IJSRuntime JS

@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Call void function
            await JS.InvokeVoidAsync("alert", "Hello from Blazor!");

            // Call function with return value
            var result = await JS.InvokeAsync<string>("localStorage.getItem", "key");

            // Call custom function
            await JS.InvokeVoidAsync("myApp.initialize", elementRef);
        }
    }
}
```

**JavaScript file (wwwroot/js/app.js)**:
```javascript
window.myApp = {
    initialize: function(element) {
        console.log("Initializing", element);
    },

    getData: function() {
        return { value: 42 };
    }
};
```

---

### Calling Blazor from JavaScript

```csharp
// Component
public class InteropComponent : ComponentBase
{
    private DotNetObjectReference<InteropComponent>? dotNetHelper;

    protected override void OnInitialized()
    {
        dotNetHelper = DotNetObjectReference.Create(this);
    }

    [JSInvokable]
    public void CallMeFromJS(string message)
    {
        Console.WriteLine($"Called from JS: {message}");
        StateHasChanged(); // Re-render if needed
    }

    [JSInvokable]
    public Task<string> GetDataFromBlazor()
    {
        return Task.FromResult("Data from Blazor");
    }

    public void Dispose()
    {
        dotNetHelper?.Dispose();
    }
}
```

**JavaScript**:
```javascript
// Call static method
DotNet.invokeMethodAsync('MyApp', 'StaticMethod', 'arg1');

// Call instance method
dotNetHelper.invokeMethodAsync('CallMeFromJS', 'Hello');
```

---

## 8. Accessibility (WCAG 2.1 AA)

### Semantic HTML

```razor
<nav aria-label="Main navigation">
    <FluentNavMenu>
        <FluentNavLink Href="/">Home</FluentNavLink>
        <FluentNavLink Href="/about">About</FluentNavLink>
    </FluentNavMenu>
</nav>

<main>
    <h1>Page Title</h1>
    <article>
        <h2>Article Heading</h2>
        <p>Content...</p>
    </article>
</main>

<aside aria-label="Sidebar">
    <h3>Related Links</h3>
    <ul>
        <li><a href="/link1">Link 1</a></li>
    </ul>
</aside>
```

---

### ARIA Attributes

```razor
@* Button with accessible label *@
<FluentIconButton
    Icon="@(new Icons.Regular.Size20.Delete())"
    aria-label="Delete item"
    OnClick="DeleteItem" />

@* Form with descriptive text *@
<FluentTextField
    @bind-Value="email"
    Label="Email"
    aria-describedby="email-help" />
<span id="email-help">We'll never share your email.</span>

@* Hidden decorative icon *@
<FluentIcon Icon="Icons.Regular.Size20.Info" aria-hidden="true" />

@* Live region for dynamic updates *@
<div aria-live="polite" aria-atomic="true">
    @statusMessage
</div>
```

---

### Keyboard Navigation

```razor
@* All Fluent UI components support keyboard navigation *@

@* Custom component with keyboard support *@
<div
    tabindex="0"
    @onkeydown="HandleKeyDown"
    role="button"
    aria-label="Custom button">
    Click or press Enter
</div>

@code {
    private void HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" || e.Key == " ")
        {
            // Activate on Enter or Space
            Activate();
        }
        else if (e.Key == "Escape")
        {
            // Close on Escape
            Close();
        }
    }
}
```

---

## 9. SignalR & Real-Time (Blazor Server)

### Hub Definition

```csharp
// NotificationHub.cs
using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    public async Task SendNotification(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", user, message);
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task SendToGroup(string groupName, string message)
    {
        await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
    }
}

// Program.cs
builder.Services.AddSignalR();
app.MapHub<NotificationHub>("/notificationhub");
```

---

### Component with SignalR

```razor
@inject NavigationManager Navigation
@implements IAsyncDisposable

<h3>Real-Time Notifications</h3>

@foreach (var notification in notifications)
{
    <FluentCard>@notification</FluentCard>
}

@code {
    private HubConnection? hubConnection;
    private List<string> notifications = new();

    protected override async Task OnInitializedAsync()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/notificationhub"))
            .WithAutomaticReconnect()
            .Build();

        hubConnection.On<string, string>("ReceiveNotification", (user, message) =>
        {
            notifications.Add($"{user}: {message}");
            StateHasChanged();
        });

        await hubConnection.StartAsync();
    }

    private async Task SendNotification()
    {
        if (hubConnection is not null)
        {
            await hubConnection.SendAsync("SendNotification", "User", "Hello!");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}
```

---

## 10. Testing with bUnit

### Basic Component Test

```csharp
using Bunit;
using FluentAssertions;
using Xunit;

public class CounterTests : TestContext
{
    [Fact]
    public void Counter_StartsAtZero()
    {
        // Arrange & Act
        var cut = RenderComponent<Counter>();

        // Assert
        cut.Find("p").TextContent.Should().Contain("count: 0");
    }

    [Fact]
    public void Counter_Increments_OnButtonClick()
    {
        // Arrange
        var cut = RenderComponent<Counter>();
        var button = cut.Find("button");

        // Act
        button.Click();

        // Assert
        cut.Find("p").TextContent.Should().Contain("count: 1");
    }
}
```

---

### Testing with Parameters

```csharp
[Fact]
public void Component_Renders_WithParameters()
{
    // Arrange & Act
    var cut = RenderComponent<MyComponent>(parameters => parameters
        .Add(p => p.Title, "Test Title")
        .Add(p => p.Count, 5));

    // Assert
    cut.Find("h3").TextContent.Should().Be("Test Title");
    cut.Find("p").TextContent.Should().Contain("5");
}
```

---

### Testing EventCallbacks

```csharp
[Fact]
public void Component_Invokes_Callback()
{
    // Arrange
    var callbackInvoked = false;
    var cut = RenderComponent<MyComponent>(parameters => parameters
        .Add(p => p.OnClick, () => callbackInvoked = true));

    // Act
    cut.Find("button").Click();

    // Assert
    callbackInvoked.Should().BeTrue();
}
```

---

### Testing with Services

```csharp
[Fact]
public void Component_Uses_InjectedService()
{
    // Arrange
    var mockService = new Mock<IDataService>();
    mockService.Setup(s => s.GetData()).Returns(new List<Item> { new() { Name = "Test" } });

    Services.AddSingleton(mockService.Object);

    // Act
    var cut = RenderComponent<DataComponent>();

    // Assert
    cut.FindAll("li").Should().HaveCount(1);
    cut.Find("li").TextContent.Should().Be("Test");
}
```

---

## Common Patterns Summary

| Pattern | Key Concept | Example |
|---------|-------------|---------|
| **Component Parameters** | `[Parameter]` property | Parent-to-child data flow |
| **EventCallback** | `EventCallback<T>` parameter | Child-to-parent communication |
| **Cascading Values** | `<CascadingValue>` + `[CascadingParameter]` | Share data down tree |
| **State Service** | Scoped service with events | Global state management |
| **EditForm** | `<EditForm Model>` | Form with validation |
| **JavaScript Interop** | `IJSRuntime.InvokeAsync` | Call JS from Blazor |
| **SignalR** | `HubConnection` | Real-time communication |
| **bUnit Testing** | `RenderComponent<T>()` | Component unit tests |

---

## Best Practices

1. **Use Fluent UI components** for consistent, accessible UI
2. **Dispose resources** in `Dispose()`/`DisposeAsync()`
3. **Async all the way** - Prefer `OnInitializedAsync` over synchronous methods
4. **EventCallback for events** - Not regular events
5. **Services for shared state** - With change notification pattern
6. **Semantic HTML** - Use proper elements for accessibility
7. **ARIA labels** - For icon buttons and dynamic content
8. **Test components** - Use bUnit for component logic tests
9. **SignalR reconnection** - Use `WithAutomaticReconnect()`
10. **Route constraints** - Validate route parameters at routing level

---

**Next**: See `REFERENCE.md` for comprehensive guides, advanced patterns, and production deployment strategies.
