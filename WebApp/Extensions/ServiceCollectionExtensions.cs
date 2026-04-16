using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Progress.Sitefinity.AspNetCore.Widgets.Models.Profile;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Vfl.Renderer.Models.LoginStatus;
using Vfl.Renderer.Models.StaticSection;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Config;
using VFL.Renderer.Models.Dashboard;
using VFL.Renderer.Models.HeroBannerSlide;
using VFL.Renderer.Models.LoginForm;
using VFL.Renderer.Models.PageTitle;
using VFL.Renderer.Models.PricePackage;
using VFL.Renderer.Models.Profile;
using VFL.Renderer.Models.Registration;
using VFL.Renderer.Models.ResetPassword;
using VFL.Renderer.Models.VitiProduct;
using VFL.Renderer.Services.Dashboard;
using VFL.Renderer.Services.DirectTopUp;
using VFL.Renderer.Services.LoginService;
using VFL.Renderer.Services.Plans;
using VFL.Renderer.Services.Profile;
using VFL.Renderer.Services.Registration;
using VFL.Renderer.Services.ResetPasswordService;
using VFL.Renderer.Services.TransactionHistroy;
using VFL.Renderer.Services.Validation;
using VFL.Renderer.ViewComponents;
using VFL.Renderer.Services.MyBillProfile;
using VFL.Renderer.Services.MyBillLogin;
using VFL.Renderer.Models.MyBillResetPassword;
using VFL.Renderer.Services.MyBillResetPasswordService;
using VFL.Renderer.Models.MyBillLoginForm;
using VFL.Renderer.Models.MyBillLoginStatus;
using VFL.Renderer.Models.MyBillDashboard;
using VFL.Renderer.Models.MyBillProfileSettings;
using VFL.Renderer.Models.CTA;
using VFL.Renderer.Models.DirectTopUp;
using VFL.Renderer.Services.MyBillPayment;
using VFL.Renderer.Services.MyBillTransactionHistory;
using VFL.Renderer.Models.MyBillTransactionHistory;
using VFL.Renderer.Models.MyBillCheckout;

namespace VFL.Renderer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            // Add memory cache
            services.AddMemoryCache();

            // Retry + CircuitBreaker
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

            var circuitBreakerPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30));

            // HttpClient with policies
            services.AddHttpClient<IApiClient, ApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://testonlineservices.vodafone.com.fj:4343/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);

            // Services
            services.AddHttpClient<RegistrationApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://testonlineservices.vodafone.com.fj:4343/");
            });

            // NOTE: Do NOT call AddAuthentication() here — it would overwrite the
            // default scheme (Cookies / MyBillAuth) already configured in Program.cs.
            // Only register the JwtBearer scheme as an additional handler.
            services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Cookies["SitefinityJwtAuth"];   
                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            // MyVodafone Services (unchanged)
            services.AddScoped<IHeroBannerSlideModel, HeroBannerSlideModel>();
            services.AddScoped<ILoginStatusModel, LoginStatusModel>();
            services.AddScoped<IVitiProductModel, VitiProductModel>();
            services.AddScoped<IPricePackageModel, PricePackageModel>();
            services.AddScoped<IRegistrationModel, CustomRegistrationModel>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<ICustomLoginFormModel, CustomLoginFormModel>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IStaticSection, StaticSection>();
            services.AddScoped<IApiClient, ApiClient>();
            services.AddScoped<ICustomResetPasswordModel, CustomResetPasswordModel>();
            services.AddScoped<IResetPasswordService, ResetPasswordService>();
            services.AddScoped<RegistrationApiClient>();
            services.AddScoped<SitefinityServiceClient>();            
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IProfileModel, ProfileModel>();
            services.AddScoped<CustomProfileViewComponent>();
            services.AddScoped<ICustomProfileModel, CustomProfileModel>();
            services.AddScoped<IPageTitleModel, PageTitleModel>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IDashboardModel, DashboardModel>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IDirectTopUpService, DirectTopUpService>();
            services.AddScoped<IDirectTopUpModel, DirectTopUpModel>(); 
            services.AddScoped<DirectTopUpService>();
            services.AddScoped<ITransactionHistoryService, TransactionHistoryService>();
            services.AddScoped<IPlansService, PlansService>();
            services.AddScoped<ICTAModel, CTAModel>();
            services.AddScoped<WebClient>();

            // MyBill Services
            services.AddScoped<IAuthServiceMyBill, AuthServiceMyBill>();
            services.AddScoped<ICustomMyBillResetPasswordModel, CustomMyBillResetPasswordModel>();
            services.AddScoped<IMyBillResetPasswordService, MyBillResetPasswordService>();
            services.AddScoped<ICustomMyBillLoginModel, CustomMyBillLoginModel>();
            services.AddScoped<IMyBillLoginStatusModel, MyBillLoginStatusModel>();
            services.AddScoped<IMyBillDashboardModel, MyBillDashboardModel>();
            services.AddScoped<IMyBillProfileService, MyBillProfileService>();
            services.AddScoped<IMyBillProfileSettingsModel, MyBillProfileSettingsModel>();
            services.AddScoped<IMyBillPaymentService, MyBillPaymentService>();
            services.AddScoped<IMyBillTransactionHistoryService, MyBillTransactionHistoryService>();
            services.AddScoped<IMyBillTransactionHistoryModel, MyBillTransactionHistoryModel>();

            // MyBill WebClient (separate instance for MyBill portal)
            // UseCookies = false is CRITICAL: without it, HttpClientHandler silently consumes
            // Set-Cookie headers from backend responses, preventing refresh token cookies
            // from being forwarded to the browser.
            services.AddHttpClient<MyBillWebClient>()
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    UseCookies = false
                });
            
            // MyBill Public API Client (for unauthenticated operations like forgot password)
            services.AddHttpClient<MyBillPublicApiClient>();

            // MyBill Checkout Session Store
            services.AddSingleton<ICheckoutSessionStore, CheckoutSessionStore>();

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<WebClient>>();
            services.AddSingleton(typeof(ILogger), logger);

            return services;
        }
    }
}