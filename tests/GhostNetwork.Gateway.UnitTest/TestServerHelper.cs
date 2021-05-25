using System;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GhostNetwork.Gateway.UnitTest
{
    public static class TestServerHelper
    {
        public static HttpClient New(Action<IServiceCollection> configureServices)
        {
            var server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureTestServices(configureServices));

            return server.CreateClient();
        }

        public static StringContent AsJsonContent<T>(this T input)
        {
            return new StringContent(JsonSerializer.Serialize(input), Encoding.Default, "application/json");
        }
    }
    
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "Test user") };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}