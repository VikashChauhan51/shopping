using Ecart.Core.Handlers;
using Shopping.Web.Resilience;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<PollyPolicies>();
builder.Services.AddTransient<LoggingDelegatingHandler>();
builder.Services.AddRefitClient<ICatalogService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var policies = serviceProvider.GetRequiredService<PollyPolicies>();
        return policies.GetRetryPolicy();
    })
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var policies = serviceProvider.GetRequiredService<PollyPolicies>();
        return policies.GetCircuitBreakerPolicy();
    });
builder.Services.AddRefitClient<IBasketService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var policies = serviceProvider.GetRequiredService<PollyPolicies>();
        return policies.GetRetryPolicy();
    })
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var policies = serviceProvider.GetRequiredService<PollyPolicies>();
        return policies.GetCircuitBreakerPolicy();
    });

builder.Services.AddRefitClient<IOrderingService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var policies = serviceProvider.GetRequiredService<PollyPolicies>();
        return policies.GetRetryPolicy();
    })
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var policies = serviceProvider.GetRequiredService<PollyPolicies>();
        return policies.GetCircuitBreakerPolicy();
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
