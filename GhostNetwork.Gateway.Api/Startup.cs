using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using Azure.Storage.Blobs;
using GhostNetwork.Content.Api;
using GhostNetwork.Education.Api;
using GhostNetwork.Gateway.Api.Helpers;
using GhostNetwork.Gateway.Chats;
using GhostNetwork.Gateway.Infrastructure;
using GhostNetwork.Gateway.Infrastructure.SecuritySettingResolver;
using GhostNetwork.Gateway.Messages;
using GhostNetwork.Gateway.NewsFeed;
using GhostNetwork.Gateway.Users;
using GhostNetwork.Messages.Api;
using GhostNetwork.Profiles.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Gateway.Api
{
    public class Startup
    {
        private const string ApiName = "api";
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.configuration = configuration;
            this.environment = environment;
        }

        private Uri Authority => new Uri(configuration.GetValue("AUTHORITY", "https://accounts.ghost-network.boberneprotiv.com"));

        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = configuration.GetValue("SHOW_PII", false);

            services.AddScoped(_ => new FeatureFlags(configuration.GetValue("ENABLE_PERSONALIZED_NEWSFEED", false), GetPersonalizedNewsfeed()));

            services.AddCors();
            services.AddHttpContextAccessor();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(ApiName, new OpenApiInfo
                {
                    Title = "GhostNetwork/Gateway API",
                    Version = "1.5.0"
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
                    options.RequireHttpsMetadata = configuration.GetValue("AUTHORITY_REQUIRE_HTTPS", true);
                });

            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

            services.AddTransient<SecuritySettingsFriendsResolver>();
            services.AddTransient<SecuritySettingsFollowersResolver>();
            services.AddTransient<SecuritySettingsPublicationResolver>();

            if (configuration["FILE_STORAGE_TYPE"]?.ToLower() == "local")
            {
                services.AddScoped<IUsersPictureStorage, UsersPictureLocalStorage>(provider => new UsersPictureLocalStorage(
                    environment.WebRootPath,
                    $"{provider.GetRequiredService<IHttpContextAccessor>().HttpContext!.Request.Scheme}://{provider.GetRequiredService<IHttpContextAccessor>().HttpContext!.Request.Host.Value}",
                    provider.GetRequiredService<IProfilesApi>()));
            }
            else
            {
                services.AddScoped<IUsersPictureStorage, UsersPictureStorage>(provider => new UsersPictureStorage(
                    new BlobServiceClient(configuration["BLOB_CONNECTION"]),
                    provider.GetRequiredService<IProfilesApi>()));
            }

            services.AddTransient<LoggingHttpHandler>();
            services.AddTransient<SetRequestIdHttpHandler>();

            services.AddHttpClient("content")
                .AddHttpMessageHandler<LoggingHttpHandler>()
                .AddHttpMessageHandler<SetRequestIdHttpHandler>();
            services.AddScoped<IPublicationsApi>(provider => new PublicationsApi(provider.GetRequiredService<IHttpClientFactory>().CreateClient("content"), configuration["CONTENT_ADDRESS"]));
            services.AddScoped<ICommentsApi>(provider => new CommentsApi(provider.GetRequiredService<IHttpClientFactory>().CreateClient("content"), configuration["CONTENT_ADDRESS"]));
            services.AddScoped<IReactionsApi>(provider => new ReactionsApi(provider.GetRequiredService<IHttpClientFactory>().CreateClient("content"), configuration["CONTENT_ADDRESS"]));

            services.AddHttpClient("profile")
                .AddHttpMessageHandler<LoggingHttpHandler>()
                .AddHttpMessageHandler<SetRequestIdHttpHandler>();
            services.AddScoped<IProfilesApi>(provider => new ProfilesApi(provider.GetRequiredService<IHttpClientFactory>().CreateClient("profile"), configuration["PROFILES_ADDRESS"]));
            services.AddScoped<IRelationsApi>(provider => new RelationsApi(provider.GetRequiredService<IHttpClientFactory>().CreateClient("profile"), configuration["PROFILES_ADDRESS"]));
            services.AddScoped<ISecuritySettingsApi>(provider => new SecuritySettingsApi(provider.GetRequiredService<IHttpClientFactory>().CreateClient("profile"), configuration["PROFILES_ADDRESS"]));

            services.AddHttpClient("messaging")
                .AddHttpMessageHandler<LoggingHttpHandler>()
                .AddHttpMessageHandler<SetRequestIdHttpHandler>();
            services.AddScoped<IChatsApi>(provider => new ChatsApi(provider.GetRequiredService<IHttpClientFactory>().CreateClient("messaging"), configuration["MESSAGES_ADDRESS"]));
            services.AddScoped<IMessagesApi>(provider => new MessagesApi(provider.GetRequiredService<IHttpClientFactory>().CreateClient("messaging"), configuration["MESSAGES_ADDRESS"]));

            services.AddScoped<NewsFeedApi>()
                .AddHttpClient<NewsFeedApi>(c => c.BaseAddress = new Uri(configuration["NEWSFEED_ADDRESS"]))
                .AddHttpMessageHandler<LoggingHttpHandler>()
                .AddHttpMessageHandler<SetRequestIdHttpHandler>();
            services.AddScoped<INewsFeedStorage, NewsFeedStorage>();

            services.AddHttpClient("education")
                .AddHttpMessageHandler<LoggingHttpHandler>()
                .AddHttpMessageHandler<SetRequestIdHttpHandler>();
            services.AddScoped<IFlashCardsApi>(provider => new FlashCardsApi(provider.GetRequiredService<IHttpClientFactory>().CreateClient("education"), configuration["EDUCATION_ADDRESS"]));

            services.AddScoped<RestUsersStorage>();
            services.AddScoped<IUsersStorage, RestUsersStorage>();
            services.AddScoped<ISecuritySettingStorage, SecuritySettingsStorage>();

            services.AddScoped<IChatStorage, ChatStorage>();
            services.AddScoped<ChatValidator>();

            services.AddScoped<IMessageStorage, MessagesStorage>();
            services.AddScoped<MessageValidator>();
            services.AddScoped(provider => new ContextProvider(
                provider.GetRequiredService<IHttpContextAccessor>().HttpContext!.Request
                    .Headers[Consts.Headers.RequestId].FirstOrDefault()
                ?? Guid.NewGuid().ToString()));

            services.AddControllers()
                .AddJsonOptions(options => options
                    .JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<LoggingMiddleware>();
            if (env.IsDevelopment())
            {
                app
                    .UseSwagger()
                    .UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint($"/swagger/{ApiName}/swagger.json", "Gateway V1");

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
                        options.SwaggerEndpoint($"/swagger/{ApiName}/swagger.json", "Gateway V1");

                        options.OAuthClientId("swagger_prod");
                        options.OAuthClientSecret("secret");
                        options.OAuthAppName("Swagger Prod");
                        options.OAuthUsePkce();
                    });
            }

            if (configuration["FILE_STORAGE_TYPE"]?.ToLower() == "local")
            {
                app.UseStaticFiles();
            }

            app.UseRouting();

            app.UseCors(builder =>
            {
                var allowOrigins = GetAllowOrigins();
                builder = allowOrigins.Any()
                    ? builder.WithOrigins(allowOrigins)
                    : builder.AllowAnyOrigin();

                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders(Consts.Headers.All);
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private string[] GetAllowOrigins()
        {
            return configuration.GetValue<string>("ALLOWED_HOSTS")?.Split(',').ToArray() ?? Array.Empty<string>();
        }

        private string[] GetPersonalizedNewsfeed()
        {
            return configuration.GetValue<string>("ENABLE_PERSONALIZED_NEWSFEED_FOR")?.Split(',').ToArray() ?? Array.Empty<string>();
        }
    }
}
