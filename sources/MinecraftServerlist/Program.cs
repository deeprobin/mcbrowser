using Microsoft.EntityFrameworkCore;
using MinecraftServerlist;
using MinecraftServerlist.Data.Development;
using MinecraftServerlist.Data.Infrastructure;
using MinecraftServerlist.InternalApi.Controllers;
using MinecraftServerlist.InternalApi.Extensions;
using MinecraftServerlist.PublicApi;
using MinecraftServerlist.Services.Abstractions;
using MinecraftServerlist.Services.Extensions;
using MinecraftServerlist.Web.Extensions;
using Prometheus;
using Prometheus.SystemMetrics;

var builder = WebApplication.CreateBuilder(args);

if (Convert.ToBoolean(builder.Configuration["DevelopmentDatabase"]))
{
    // Use dummy data
    builder.Services.AddDbContext<PostgresDbContext>(options => options.UseInMemoryDatabase("DevDatabase"));
}
else
{
    // Use Postgres Database
    builder.Services.AddDbContext<PostgresDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DbContext")));
}

builder.Services.AddHttpClient();
builder.Services.AddInternalApiServices();
builder.Services.AddPublicApiServices();
builder.Services.AddServices();
//builder.Services.AddPrometheusAspNetCoreMetrics();
//builder.Services.AddPrometheusEntityFrameworkMetrics();
//builder.Services.AddPrometheusHttpClientMetrics();
//builder.Services.AddSystemMetrics();
builder.Services.AddControllersWithViews();
builder.Services.MapBlazorFrontend("");
//builder.Services.AddRazorPages();

var app = builder.Build();
using var scope = app.Services.CreateScope();

if (Convert.ToBoolean(app.Configuration["DevelopmentDatabase"]))
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
    DataSeeder.SeedData(dbContext);
}

var baseUrl = app.Configuration["BaseUrl"];

if (app.Environment.IsDevelopment())
{
    var httpsUrl = app.Urls.FirstOrDefault(s => s.StartsWith("https://"));
    baseUrl ??= httpsUrl;
}

var url = baseUrl;
if (baseUrl is not null)
{
    if (!url!.EndsWith('/'))
    {
        url += '/';
    }

    url += $"api/internal/{nameof(PaymentController)}/{nameof(PaymentController.StripeWebHook)}/";
}

var canListenToWebHook = false;
StripeHandle? stripeHandle = default;

if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("No Stripe WebHook is registered because this instance runs in Development-Environment.");
    if (StripeHandle.IsStripeInstalled())
    {
        canListenToWebHook = true;
        stripeHandle = new StripeHandle(url);
        app.Logger.LogInformation("Forwarding Stripe-Events to WebHooks to url {Url}", url);
    }
    else
    {
        app.Logger.LogWarning("Cannot listen for stripe events: Stripe-CLI is not installed - Please install Stripe CLI https://stripe.com/docs/stripe-cli");
    }
}

if (baseUrl is null)
{
    app.Logger.LogWarning("Cannot register Stripe WebHook: Missing configuration variable BaseUrl");
    canListenToWebHook = false;
}

if (canListenToWebHook)
{
    var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

    app.Logger.LogInformation("Registering Stripe WebHook to {Url}", url);
    await paymentService.MapWebHookAsync(url);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

//app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

//app.UseMetricServer();
//app.UseHttpMetrics();
app.MapInternalApi();
app.MapPublicApi();
app.MapBlazorHub(); // _blazor
app.MapRazorPages();

//app.MapFallbackToFile("index.html");
//app.MapFallbackToFile("/_Host");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Logger.LogInformation("Preparing intrinsic methods");
var runtimeService = app.Services.GetRequiredService<IRuntimeService>();
runtimeService.PrepareIntrinsics();

app.Logger.LogInformation("Initialization done");
app.Run();
stripeHandle?.Dispose();