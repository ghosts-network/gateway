using System;
using System.Collections.Generic;
using GhostNetwork.Gateway.Api.Users;
using GhostNetwork.Profiles.Api;
using GhostNetwork.Publications.Api;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Gateway.Api
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddHttpContextAccessor();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "GhostNetwork/Gateway API",
                    Version = "1.0.0"
                });

                options.OperationFilter<AddResponseHeadersFilter>();
                options.OperationFilter<AuthorizeCheckOperationFilter>();

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://account.gn.boberneprotiv.com/connect/authorize"),
                            TokenUrl = new Uri("https://account.gn.boberneprotiv.com/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                {
                                    "api", "Main API - full access"
                                }
                            }
                        }
                    }
                });
            });

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    options.ApiName = "api";
                    options.Authority = "https://account.gn.boberneprotiv.com";
                });

            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            services.AddSingleton(provider => GrpcChannel.ForAddress(configuration["PROFILES_GRPC_ADDRESS"]));
            services.AddScoped(provider => new Profiles.Grpc.Profiles.ProfilesClient(provider.GetRequiredService<GrpcChannel>()));

            services.AddScoped<IPublicationsApi>(provider => new PublicationsApi(configuration["PUBLICATIONS_ADDRESS"]));
            services.AddScoped<ICommentsApi>(provider => new CommentsApi(configuration["PUBLICATIONS_ADDRESS"]));
            services.AddScoped<IReactionsApi>(provider => new ReactionsApi(configuration["PUBLICATIONS_ADDRESS"]));
            services.AddScoped<IProfilesApi>(provider => new ProfilesApi(configuration["PROFILES_ADDRESS"]));

            var i = 0;
            services.AddScoped<GrpcUsersStorage>();
            services.AddScoped<RestUsersStorage>();
            services.AddScoped<IUsersStorage>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<IUsersStorage>>();
                i = (i + 1) % 2;
                if (i % 2 == 0)
                {
                    logger.LogInformation("GRPC storage");
                    return provider.GetRequiredService<GrpcUsersStorage>();
                }

                logger.LogInformation("REST storage");
                return provider.GetRequiredService<RestUsersStorage>();
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app
                    .UseSwagger()
                    .UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway V1");

                        options.OAuthClientId("swagger_local");
                        options.OAuthClientSecret("secret");
                        options.OAuthAppName("Swagger Local");
                        options.OAuthUsePkce();
                    });
            }
            else
            {
                app
                    .UseSwagger()
                    .UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway V1");

                        options.OAuthClientId("swagger_prod");
                        options.OAuthClientSecret("secret");
                        options.OAuthAppName("Swagger Prod");
                        options.OAuthUsePkce();
                    });
            }

            app.UseRouting();

            app.UseCors(builder => builder
                .WithOrigins("http://localhost:4200", "https://gn.boberneprotiv.com")
                .AllowAnyHeader()
                .WithExposedHeaders(Consts.Headers.All)
                .AllowAnyMethod());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}