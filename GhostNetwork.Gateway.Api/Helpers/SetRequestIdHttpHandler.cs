using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Api.Helpers;

public class SetRequestIdHttpHandler : DelegatingHandler
{
    private readonly ContextProvider contextProvider;

    public SetRequestIdHttpHandler(ContextProvider contextProvider)
    {
        this.contextProvider = contextProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add(Consts.Headers.RequestId, contextProvider.CorrelationId);
        return await base.SendAsync(request, cancellationToken);
    }
}