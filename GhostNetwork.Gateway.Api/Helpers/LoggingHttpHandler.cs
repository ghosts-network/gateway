using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GhostNetwork.Gateway.Api.Helpers;

public class LoggingHttpHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHttpHandler> logger;

    public LoggingHttpHandler(ILogger<LoggingHttpHandler> logger)
    {
        this.logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using (logger.BeginScope(new Dictionary<string, object>
               {
                   ["type"] = "outgoing:http"
               }))
        {
            logger.LogInformation($"Outgoing http request started {request.Method} {request.RequestUri!.PathAndQuery}");

            var sw = Stopwatch.StartNew();
            var result = await base.SendAsync(request, cancellationToken);

            sw.Stop();
            using (logger.BeginScope(new Dictionary<string, object>
                   {
                       ["elapsedMilliseconds"] = sw.ElapsedMilliseconds
                   }))
            {
                logger.LogInformation($"Outgoing http request finished {request.Method} {request.RequestUri!.PathAndQuery}");
            }

            return result;
        }
    }
}