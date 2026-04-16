using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Progress.Sitefinity.AspNetCore;
using Progress.Sitefinity.AspNetCore.FormWidgets;
using Serilog;
using Serilog.Debugging;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Vfl.Renderer.Models.LoginStatus;
using Vfl.Renderer.Models.StaticSection;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Config;
using VFL.Renderer.Extensions;
using VFL.Renderer.Models.Checkout;
using VFL.Renderer.Models.Dashboard;
using VFL.Renderer.Models.HeroBannerSlide;
using VFL.Renderer.Models.LoginForm;
using VFL.Renderer.Models.PricePackage;
using VFL.Renderer.Models.PurchasePlan;
using VFL.Renderer.Models.Registration;
using VFL.Renderer.Models.Subscription;
using VFL.Renderer.Models.VitiProduct;
using VFL.Renderer.Models.WebTopUp;
using VFL.Renderer.Services.Dashboard;
using VFL.Renderer.Services.PrepayGifting;
using VFL.Renderer.Services.Profile;
using VFL.Renderer.Services.Subscription;
using VFL.Renderer.Services.WebTopUp;

var builder = WebApplication.CreateBuilder(args);

// show sink/init errors during startup
SelfLog.Enable(msg => Console.Error.WriteLine(msg));

builder.Logging.ClearProviders();
builder.Host.UseSerilog();

// CRITICAL: Add Data Protection for cookie encryption
// This ensures cookies can be encrypted/decrypted consistently across requests
builder.Services.AddDataProtection()
    .SetApplicationName("VFL.Renderer")
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "keys")));

// Bind ApiSettings section to ApiSettings class and add to DI container
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Register HttpClient for AuthApiClient with DI
builder.Services.AddHttpClient<AuthApiClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(100);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        AllowAutoRedirect = true,
        UseCookies = false,
        MaxConnectionsPerServer = 10
    };

    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback =
            (message, cert, chain, errors) => true;
    }

    return handler;
});

// Add HttpContextAccessor for dependency injection
builder.Services.AddHttpContextAccessor();

// Add session configuration (MUST come before AddAuthentication)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10080); // 7 days - matches refresh token
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Required for GDPR compliance
    options.Cookie.Name = ".MyBill.Session";
});

builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<DashboardModel>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<CustomLoginFormModel>();

// Register memory cache for AuthServiceMyBill
builder.Services.AddMemoryCache();

// Add authentication services
// Configure all authentication schemes in a single call to avoid overwriting
builder.Services.AddAuthentication(options =>
{
    // Default scheme is for MyVodafone (regular Sitefinity)
    // NOTE: When accessing MyBill endpoints, the [Authorize(AuthenticationSchemes = "MyBillAuth")]
    // attribute will override this and use MyBillAuth instead
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.Cookie.Name = "SitefinityJwtAuth";  // MyVodafone authentication cookie
    options.Cookie.Path = "/";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
})
.AddCookie("MyBillAuth", options =>
{
    options.LoginPath = "/my-bill-login";
    options.LogoutPath = "/mybill-logout";
    options.Cookie.Name = "MyBillAuthCookie";  // MyBill authentication cookie
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest; // Allow both HTTP and HTTPS in dev
    options.Cookie.Path = "/";  // Ensure cookie is available across entire site

    // Cookie lifetime MUST match the backend refresh token expiry.
    // Read from appsettings (MyBillRefreshTokenExpiryMinutes = 10080 = 7 days).
    // SlidingExpiration is OFF — the cookie lives for the full duration set at
    // sign-in time. The JWT inside it (14 min) is refreshed transparently by
    // mybill-auth-refresh.js and MyBillWebClient server-side; the cookie itself
    // just needs to survive until the refresh token expires.
    var myBillSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>();
    options.ExpireTimeSpan = TimeSpan.FromMinutes(myBillSettings?.MyBillRefreshTokenExpiryMinutes ?? 10080);
    options.SlidingExpiration = false;

    options.Events = new CookieAuthenticationEvents
    {
        OnValidatePrincipal = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

            // Check if the access_tokenMyBill claim exists and is not empty
            var tokenClaim = context.Principal?.FindFirst("access_tokenMyBill");
            if (tokenClaim == null || string.IsNullOrEmpty(tokenClaim.Value))
            {
                logger.LogDebug("MyBill Auth: Cookie exists but access_tokenMyBill claim is missing or empty, rejecting principal");
                context.RejectPrincipal();
                return Task.CompletedTask;
            }

            // NOTE: Do NOT manually check ExpiresUtc here.
            // ASP.NET Core's cookie auth handler already rejects expired cookies
            // before OnValidatePrincipal fires. A manual check interfered with
            // SlidingExpiration because the handler renews ExpiresUtc AFTER this
            // callback, causing valid cookies to be incorrectly rejected.

            return Task.CompletedTask;
        },
        OnSignedIn = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogDebug("MyBill Auth: User {User} signed in at {Time}",
                context.Principal?.Identity?.Name ?? "Unknown",
                DateTimeOffset.UtcNow);
            return Task.CompletedTask;
        },
        OnSigningOut = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogDebug("MyBill Auth: User signing out at {Time}", DateTimeOffset.UtcNow);
            return Task.CompletedTask;
        }
    };
});
//.AddGoogle(googleOptions =>
//{
//    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//    googleOptions.CallbackPath = "/signin-google";
//    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
//    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
//    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
//    googleOptions.SaveTokens = true;
//});

builder.Services.AddAppServices();

// Add services to the container.
builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddSitefinity();
builder.Services.AddViewComponentModels();
builder.Services.AddFormViewComponentModels();

// Add Sitefinity HttpClient with relaxed SSL validation in Development
builder.Services.AddHttpClient("Sitefinity", client =>
{
    client.BaseAddress = new Uri("https://testfprn.vodafone.com.fj:4343");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback =
            (message, cert, chain, errors) => true;
    }
    return handler;
});

builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<DashboardModel>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<WebTopUpService>();
builder.Services.AddScoped<CustomLoginFormModel>();
builder.Services.AddScoped<WebTopUpService>();
builder.Services.AddScoped<IWebTopUpModel, WebTopUpModel>();
builder.Services.AddScoped<IPurchasePlanModel, PurchasePlanModel>();
builder.Services.AddScoped<IPrepayGiftingService, PrepayGiftingService>();
builder.Services.AddScoped<ISubscriptionModel, SubscriptionModel>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICheckoutSessionModel, CheckoutSessionModel>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

// Add session middleware (MUST come before authentication)
app.UseSession();

// CRITICAL: Add authentication & authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Custom middleware to handle MyBill authentication scheme selection
// This ensures MyBill routes AND pages use MyBillAuth scheme instead of default
app.Use(async (context, next) =>
{
    try
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var apiSettings = context.RequestServices.GetRequiredService<IOptions<ApiSettings>>().Value;

        // Check for MyBillAuthCookie existence first
        bool hasMyBillCookie = context.Request.Cookies.ContainsKey("MyBillAuthCookie");
        
        logger.LogDebug("MyBill Middleware: Path={Path}, HasCookie={HasCookie}", path, hasMyBillCookie);
        
        if (hasMyBillCookie)
        {
            // Try to authenticate using MyBillAuth scheme explicitly
            var result = await context.AuthenticateAsync("MyBillAuth");

            if (result.Succeeded && result.Principal != null)
            {
                // Verify the principal has required claims
                var tokenClaim = result.Principal.FindFirst("access_tokenMyBill");
                var hasTokenClaim = tokenClaim != null && !string.IsNullOrEmpty(tokenClaim.Value);
                
                if (hasTokenClaim)
                {
                    // Replace the default authenticated user with MyBill user
                    context.User = result.Principal;

                    // If user is already authenticated and navigates to the login page,
                    // redirect to the dashboard using the configured path from ApiSettings
                    var loginPath = (apiSettings.MyBillLoginPath ?? "/my-bill-login").ToLowerInvariant().TrimEnd('/');
                    if (path.TrimEnd('/') == loginPath)
                    {
                        var dashboardPath = apiSettings.MyBillDashboardPath ?? "/mybill/mybill-dashboard";
                        logger.LogInformation("MyBill Middleware: Authenticated user on login page, redirecting to {DashboardPath}", dashboardPath);
                        context.Response.Redirect(dashboardPath);
                        return;
                    }
                }
                else
                {
                    logger.LogDebug("MyBill Middleware: Cookie exists but access_tokenMyBill claim is missing or empty, deleting stale cookie");
                    context.Response.Cookies.Delete("MyBillAuthCookie", new CookieOptions { Path = "/" });
                }
            }
            else
            {
                // Cookie exists but authentication failed - cookie is expired or invalid
                // Delete the stale cookie so it doesn't keep triggering failed validation on every request
                logger.LogDebug("MyBill Middleware: MyBillAuthCookie exists but authentication failed, deleting stale cookie");
                context.Response.Cookies.Delete("MyBillAuthCookie", new CookieOptions { Path = "/" });
            }
        }
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "MyBill Middleware: Error in authentication middleware");
        // Continue processing even if middleware fails
    }

    await next();
});

// UseSitefinity handles endpoint mapping internally (including API controllers)
app.UseSitefinity();

app.Run();
