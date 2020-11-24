using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;
using NetCoreMicroserviceSample.Api.Controllers;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCoreMicroserviceSample.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IBusinessLogic, BusinessLogic>();

            // Cookie configuration for HTTPS
            services.Configure<CookiePolicyOptions>(options => options.MinimumSameSitePolicy = SameSiteMode.None);

            // Add HTTP client used to get tokens from identity server
            services.AddHttpClient("identity-server", c => c.BaseAddress = new Uri(Configuration["Oidc:Domain"]));

            // Add authentication services see https://identityserver4.readthedocs.io
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect("IdentityServer", options =>
            {
                // Set the authority to your domain
                options.Authority = Configuration["Oidc:Domain"];

                // Configure the Client ID and Client Secret
                options.ClientId = Configuration["Oidc:ClientId"];
                options.ClientSecret = Configuration["Oidc:ClientSecret"];

                // Set response type to code
                options.ResponseType = OpenIdConnectResponseType.Code;

                // Configure the scope
                options.Scope.Clear();
                options.Scope.Add("openid profile email");

                // Set the callback path
                options.CallbackPath = new PathString("/callback");

                // Configure the Claims Issuer to be Auth0
                options.ClaimsIssuer = Configuration["Oidc:Domain"];

                options.Events = new OpenIdConnectEvents
                {
                    OnAuthorizationCodeReceived = context =>
                    {
                        if (Debugger.IsAttached)
                        {
                            // Do NOT write access codes to logs. This is for training purposes only.
                            Debug.WriteLine($"Received code {context.TokenEndpointRequest.Code}, requesting access token");
                        }

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        if (Debugger.IsAttached)
                        {
                            // Do NOT write access tokens to logs. This is for training purposes only.
                            Debug.WriteLine($"Token {context.TokenEndpointResponse.AccessToken} validated");
                        }

                        // Get profile information and add it to claims
                        using var client = new HttpClient();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", context.TokenEndpointResponse.AccessToken);
                        var response = await client.GetAsync(new Uri($"{Configuration["Oidc:Domain"]}/connect/userinfo"));
                        response.EnsureSuccessStatusCode();
                        var profile = await response.Content.ReadFromJsonAsync<UserProfile>();
                        context.Principal!.Identities.First().AddClaim(new Claim(ClaimTypes.Email, profile!.Email));
                        context.Principal!.Identities.First().AddClaim(new Claim(ClaimTypes.Name, profile.Name));
                    },
                    OnRedirectToIdentityProviderForSignOut = (context) =>
                    {
                        // End session on identity server
                        context.Response.Redirect($"{Configuration["Oidc:Domain"]}/connect/endsession");
                        context.HandleResponse();

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NetCoreMicroserviceSample.Api", Version = "v1" });

                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
                var commentsFile = Path.Combine(baseDirectory, commentsFileName);
                c.IncludeXmlComments(commentsFile);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NetCoreMicroserviceSample.Api v1"));

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (env.IsDevelopment())
            {
                app.UseSpa(spa =>
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");
                });
            }
            else
            {
                var fp = new ManifestEmbeddedFileProvider(typeof(Startup).Assembly, "wwwroot");
                app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = fp });
                app.UseStaticFiles(new StaticFileOptions { FileProvider = fp });
            }
        }
    }
}
