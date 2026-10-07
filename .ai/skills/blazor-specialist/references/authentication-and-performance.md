# Blazor Authentication & Performance

The login page and `AuthorizeView` usage, plus virtualization and lazy-loading. Load this when adding auth to a Blazor app or optimizing a long list.

## Authentication in Blazor

```razor
@* File: Pages/Login.razor *@
@page "/login"
@inject NavigationManager Navigation
@inject IAuthenticationService AuthService

<div class="login-page">
    <h1>Login</h1>

    <EditForm Model="@loginModel" OnValidSubmit="@HandleLogin">
        <DataAnnotationsValidator />
        <ValidationSummary />

        <div class="form-group">
            <label>Email:</label>
            <InputText @bind-Value="loginModel.Email" class="form-control" />
        </div>

        <div class="form-group">
            <label>Password:</label>
            <InputText type="password" @bind-Value="loginModel.Password" class="form-control" />
        </div>

        <button type="submit" class="btn btn-primary">Login</button>
    </EditForm>
</div>

@code {
    private LoginModel loginModel = new();

    private async Task HandleLogin()
    {
        var result = await AuthService.LoginAsync(loginModel.Email, loginModel.Password);

        if (result.Success)
        {
            Navigation.NavigateTo("/");
        }
    }
}
```

```razor
@* File: Components/AuthorizeView.razor *@
@using Microsoft.AspNetCore.Components.Authorization

<AuthorizeView>
    <Authorized>
        <p>Hello, @context.User.Identity?.Name!</p>
        <a href="/logout">Logout</a>
    </Authorized>
    <NotAuthorized>
        <a href="/login">Login</a>
    </NotAuthorized>
</AuthorizeView>
```

## Performance Optimization

### Virtualization
```razor
@using Microsoft.AspNetCore.Components.Web.Virtualization

<Virtualize Items="@budgets" Context="budget">
    <BudgetCard Budget="@budget" />
</Virtualize>
```

### Lazy Loading
```razor
@* File: App.razor *@
<Router AppAssembly="@typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <PageTitle>Not found</PageTitle>
        <p>Sorry, there's nothing at this address.</p>
    </NotFound>
</Router>
```
