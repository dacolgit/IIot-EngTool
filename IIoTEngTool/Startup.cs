
using IIoTEngTool.tree;
using IIoTEngTool.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Azure.IIoT.OpcUa.Services.App.TokenStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

namespace IIoTEngTool
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            TwinServiceOptions = new TwinServiceApiOptions();
            Configuration.Bind("TwinService", TwinServiceOptions);
            AzureADOptions = new AzureADOptions();
            Configuration.Bind("AzureAD", AzureADOptions);
        }

        public IConfiguration Configuration { get; }
        public AzureADOptions AzureADOptions { get; }
        public TwinServiceApiOptions TwinServiceOptions { get; }
        public ITokenCacheService TokenCacheService { get; set; }
        public static StartupTwinServices RegistryServiceInstance { get; set; }

        // This method gets ITokenCacheService by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(AzureADOptions);
            services.AddSingleton(TwinServiceOptions);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            {
                // Without overriding the response type (which by default is id_token), the OnAuthorizationCodeReceived event is not called.
                // but instead OnTokenValidated event is called. Here we request both so that OnTokenValidated is called first which 
                // ensures that context.Principal has a non-null value when OnAuthorizeationCodeReceived is called
                options.ResponseType = "id_token code";
                
                options.Resource = TwinServiceOptions.ResourceId;

                // refresh token
                options.Scope.Add("offline_access");

                options.Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = context =>
                    {
                        // If your authentication logic is based on users then add your logic here
                        var contextAcessor = new HttpContextAccessor();
                        RegistryServiceInstance = new StartupTwinServices(AzureADOptions, TwinServiceOptions, TokenCacheService, contextAcessor);
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.Redirect("/Error");
                        context.HandleResponse(); // Suppress the exception
                        return Task.CompletedTask;
                    },
                    /// <summary>
                    /// Redeems the authorization code by calling AcquireTokenByAuthorizationCodeAsync in order to ensure
                    /// that the cache has a token for the signed-in user, which will then enable the controllers 
                    /// to call AcquireTokenSilentAsync successfully.
                    /// </summary>
                    OnAuthorizationCodeReceived = async context =>
                    {
                        // Acquire a Token for the API and cache it.
                        string userObjectId = (context.Principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;
                        var credential = new ClientCredential(context.Options.ClientId, context.Options.ClientSecret);

                        TokenCacheService = context.HttpContext.RequestServices.GetRequiredService<ITokenCacheService>();
                        var tokenCache = await TokenCacheService.GetCacheAsync(context.Principal);
                        var authContext = new AuthenticationContext(context.Options.Authority, tokenCache);

                        var authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(context.TokenEndpointRequest.Code,
                            new Uri(context.TokenEndpointRequest.RedirectUri, UriKind.RelativeOrAbsolute), credential, context.Options.Resource);

                        // Notify the OIDC middleware that we already took care of code redemption.
                        context.HandleCodeRedemption(authResult.AccessToken, context.ProtocolMessage.IdToken);
                    }
                    // If your application needs to do authenticate single users, add your user validation below.
                    //OnTokenValidated = context =>
                    //{
                    //    return myUserValidationLogic(context.Ticket.Principal);
                    //}
                };
            });

            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            // This will register IDistributedCache based token cache which ADAL will use for caching access tokens.
            services.AddScoped<ITokenCacheService, DistributedTokenCacheService>();

            //http://stackoverflow.com/questions/37371264/asp-net-core-rc2-invalidoperationexception-unable-to-resolve-service-for-type/37373557
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<GetNodeService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
