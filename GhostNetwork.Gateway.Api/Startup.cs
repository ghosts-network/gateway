using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Azure.Storage.Blobs;
using GhostNetwork.Content.Api;
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

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private Uri Authority => new Uri(configuration.GetValue("AUTHORITY", "https://accounts.ghost-network.boberneprotiv.com"));

        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = configuration.GetValue("SHOW_PII", false);

            services.AddCors();
            services.AddHttpContextAccessor();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(ApiName, new OpenApiInfo
                {
                    Title = "GhostNetwork/Gateway API",
                    Version = "1.2.1"
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

            services.AddScoped<IUsersPictureStorage, UsersPictureStorage>(provider => new UsersPictureStorage(
                new BlobServiceClient(configuration["BLOB_CONNECTION"]),
                provider.GetRequiredService<IProfilesApi>()));

            services.AddScoped<IPublicationsApi>(_ => new PublicationsApi(configuration["CONTENT_ADDRESS"]));
            services.AddScoped<ICommentsApi>(_ => new CommentsApi(configuration["CONTENT_ADDRESS"]));
            services.AddScoped<IReactionsApi>(_ => new ReactionsApi(configuration["CONTENT_ADDRESS"]));

            services.AddScoped<IProfilesApi>(_ => new ProfilesApi(configuration["PROFILES_ADDRESS"]));
            services.AddScoped<IRelationsApi>(_ => new RelationsApi(configuration["PROFILES_ADDRESS"]));
            services.AddScoped<ISecuritySettingsApi>(_ => new SecuritySettingsApi(configuration["PROFILES_ADDRESS"]));

            services.AddScoped<IChatsApi>(_ => new ChatsApi(configuration["MESSAGES_ADDRESS"]));
            services.AddScoped<IMessagesApi>(_ => new MessagesApi(configuration["MESSAGES_ADDRESS"]));

            services.AddScoped<INewsFeedStorage, NewsFeedStorage>();

            services.AddScoped<RestUsersStorage>();
            services.AddScoped<IUsersStorage, RestUsersStorage>();
            services.AddScoped<ISecuritySettingStorage, SecuritySettingsStorage>();

            services.AddScoped<IChatStorage, ChatStorage>();
            services.AddScoped<ChatValidator>();

            services.AddScoped<IMessageStorage, MessagesStorage>();
            services.AddScoped<MessageValidator>();

            services.AddControllers()
                .AddJsonOptions(options => options
                    .JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
    }
}
