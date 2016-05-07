using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using IdentityServer3.Core.Configuration;
using TeammateOnlineIdentity.Configuration;
using System.Security.Cryptography.X509Certificates;
using Owin;
using Microsoft.Owin.Security.Facebook;
using System.Net.Http;
using TeammateOnlineIdentity.UserTypes;
using Newtonsoft.Json;
using Microsoft.Owin.Security.Google;
using TeammateOnlineIdentity.Database;
using TeammateOnlineIdentity.Services;
using IdentityServer3.Core.Services;
using TeammateOnlineIdentity.Database.Repositories;

namespace TeammateOnlineIdentity
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        private readonly IApplicationEnvironment _environment;

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            _environment = appEnv;

            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();

            services.AddScoped<IUserProfileRepository, UserProfileRepository>();

            // Configure SQL connection string
            services.AddEntityFramework().AddSqlServer().AddDbContext<TeammateOnlineContext>(options =>
            {
                options.UseSqlServer(Configuration.GetSection("Database:ConnectionString").Value);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Warning;
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            // Add the platform handler to the request pipeline.
            app.UseIISPlatformHandler();
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
                app.UseRuntimeInfoPage("/runtimeinfo");
            }

            var certFile = _environment.ApplicationBasePath + $"{Path.DirectorySeparatorChar}idsrv4test.pfx";

            var idsrvOptions = new IdentityServerOptions
            {
                Factory = new IdentityServerServiceFactory()
                                .UseInMemoryUsers(Users.Get())
                                .UseInMemoryClients(Clients.Get())
                                .UseInMemoryScopes(Scopes.Get()),
                SigningCertificate = new X509Certificate2(certFile, "idsrv3test"),
                RequireSsl = false,
                SiteName = Configuration.GetSection("AppSettings:SiteTitle").Value,
                //IssuerUri = "http://teammateonline.com",
                //PublicOrigin = "http://localhost:31482",
                AuthenticationOptions = new AuthenticationOptions()
                {
                    IdentityProviders = ConfigureAdditionalProviders,
                    EnablePostSignOutAutoRedirect = true,
                    EnableSignOutPrompt = false,
                    PostSignOutAutoRedirectDelay = 0,
                }
            };

            var dbContext = app.ApplicationServices.GetService<TeammateOnlineContext>();
            var customUserService = new CustomUserService(dbContext);
            idsrvOptions.Factory.UserService = new Registration<IUserService>(resolver => customUserService);

            app.UseIdentityServer(idsrvOptions);
        }

        private void ConfigureAdditionalProviders(IAppBuilder app, string signInAsType)
        {
            var facebookAuthOptions = new FacebookAuthenticationOptions
            {
                AuthenticationType = "Facebook",
                SignInAsAuthenticationType = signInAsType,
                AppId = Configuration.GetSection("Oauth:Facebook:AppId").Value,
                AppSecret = Configuration.GetSection("Oauth:Facebook:AppSecret").Value,
                Provider = new FacebookAuthenticationProvider()
                {   
                    OnAuthenticated = (context) =>
                    {
                        using (var client = new HttpClient())
                        {
                            var result = client.GetAsync("https://graph.facebook.com/me?fields=first_name,last_name,email&access_token=" + context.AccessToken).Result;

                            if(result.IsSuccessStatusCode)
                            {
                                var userInfo = result.Content.ReadAsStringAsync().Result;
                                var facebookUser = JsonConvert.DeserializeObject<FacebookUser>(userInfo);

                                context.Identity.AddClaim(new System.Security.Claims.Claim(IdentityServer3.Core.Constants.ClaimTypes.GivenName, facebookUser.first_name));
                                context.Identity.AddClaim(new System.Security.Claims.Claim(IdentityServer3.Core.Constants.ClaimTypes.FamilyName, facebookUser.last_name));
                                context.Identity.AddClaim(new System.Security.Claims.Claim(IdentityServer3.Core.Constants.ClaimTypes.Email, facebookUser.email));
                            }
                        }

                        return Task.FromResult(0);
                    }
                },
            };
            app.UseFacebookAuthentication(facebookAuthOptions);

            var googleAuthOption = new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                SignInAsAuthenticationType = signInAsType,
                ClientId = Configuration.GetSection("Oauth:Google:ClientId").Value,
                ClientSecret = Configuration.GetSection("Oauth:Google:ClientSecret").Value,
            };
            //googleAuthOption.Scope.Add("email");
            app.UseGoogleAuthentication(googleAuthOption);
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
