using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace GhostNetwork.Gateway.Api.Helpers;

public class SetRequestIdHttpHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public SetRequestIdHttpHandler(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var contextProvider = httpContextAccessor.HttpContext!.RequestServices.GetRequiredService<ContextProvider>();
        request.Headers.Add(Consts.Headers.RequestId, contextProvider.CorrelationId);

        return await base.SendAsync(request, cancellationToken);
    }
}