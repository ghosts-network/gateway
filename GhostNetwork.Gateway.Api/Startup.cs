using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GhostNetwork.Content.Api;
using GhostNetwork.Gateway.Events;
using GhostNetwork.Gateway.Infrastructure;
using GhostNetwork.Gateway.NewsFeed;
using GhostNetwork.Gateway.RedisMq;
using GhostNetwork.Gateway.Users;
using GhostNetwork.Profiles.Api;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Gateway.Api
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly ConfigurationOptions redisConfiguration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.redisConfiguration = new ConfigurationOptions
            {
                ConnectTimeout = 5000,
                EndPoints =
                {
                    { "127.0.0.1", 50002 }
                }
            };
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
                            AuthorizationUrl = new Uri(Authority, "/connect/authorize"),
                            TokenUrl = new Uri(Authority, "/connect/token"),
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
                    options.Authority = Authority.ToString();
                });

            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            services.AddSingleton(_ => GrpcChannel.ForAddress(configuration["PROFILES_GRPC_ADDRESS"]));
            services.AddScoped(provider => new Profiles.Grpc.Profiles.ProfilesClient(provider.GetRequiredService<GrpcChannel>()));

            services.AddScoped<IPublicationsApi>(_ => new PublicationsApi(configuration["CONTENT_ADDRESS"]));
            services.AddScoped<ICommentsApi>(_ => new CommentsApi(configuration["CONTENT_ADDRESS"]));
            services.AddScoped<IReactionsApi>(_ => new ReactionsApi(configuration["CONTENT_ADDRESS"]));

            services.AddScoped<IProfilesApi>(_ => new ProfilesApi(configuration["PROFILES_ADDRESS"]));
            services.AddScoped<IRelationsApi>(_ => new RelationsApi(configuration["PROFILES_ADDRESS"]));

            services.AddScoped<INewsFeedStorage, NewsFeedStorage>();

            // services.AddScoped<GrpcUsersStorage>();
            services.AddScoped<RestUsersStorage>();
            services.AddScoped<IUsersStorage, RestUsersStorage>();

            // Redis
            services.AddSingleton<IEventBus>(provider =>
            {
                IDatabase redisDb = null;

                try
                {
                    redisDb = ConnectionMultiplexer.Connect(redisConfiguration).GetDatabase();
                }
                catch (RedisConnectionException)
                {
                    throw new ApplicationException("Redis server is unavailable");
                }

                return new EventBus(redisDb, provider);
            });

            services.AddHostedService(provider => new RedisHandlerHostedService(provider, redisConfiguration));

            // Redis handlers
            services.AddTransient<ProfileChangedEventHandler>();
 
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
                .WithExposedHeaders(Const.Headers.All)
                .AllowAnyMethod());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private Uri Authority => new Uri(configuration.GetValue("AUTHORITY", "https://account.gn.boberneprotiv.com"));
    }
}
