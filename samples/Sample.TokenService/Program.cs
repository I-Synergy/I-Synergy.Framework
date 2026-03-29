using ISynergy.Framework.AspNetCore.Abstractions.Services;
using ISynergy.Framework.AspNetCore.Authentication.Options;
using ISynergy.Framework.Core.Extensions;
using Sample.TokenService.Business;
using Sample.TokenService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(nameof(JwtOptions)).BindWithReload);

builder.Services.AddTransient<ITokenManager, TokenManager>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(c => c.Title = "Token Service Sample");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseOpenApi();
    app.UseSwaggerUi();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
