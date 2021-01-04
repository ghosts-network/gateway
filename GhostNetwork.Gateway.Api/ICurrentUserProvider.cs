using Microsoft.AspNetCore.Http;

namespace GhostNetwork.Gateway.Api
{
    public interface ICurrentUserProvider
    {
        public string UserId { get; }
    }

    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor httpContext;

        public CurrentUserProvider(IHttpContextAccessor httpContext)
        {
            this.httpContext = httpContext;
        }

        public string UserId => httpContext.HttpContext.User.FindFirst(s => s.Type == "sub").Value;
    }
}