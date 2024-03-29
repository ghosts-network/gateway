using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GhostNetwork.Gateway.Api.Helpers;

public class LoggingMiddleware
{
    private readonly RequestDelegate next;

    public LoggingMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext httpContext, ILogger<LoggingMiddleware> logger, ContextProvider contextProvider)
    {
        using (logger.BeginScope(new Dictionary<string, object>
               {
                   ["correlationId"] = contextProvider.CorrelationId
               }))
        {
            var sw = Stopwatch.StartNew();
            using (logger.BeginScope(new Dictionary<string, object>
                   {
                       ["type"] = "incoming:http",
                       ["callerId"] = httpContext.Request.Headers["X-Caller-ID"].FirstOrDefault() ?? string.Empty
                   }))
            {
                logger.LogInformation($"{httpContext.Request.Method} {httpContext.Request.Path.Value}{httpContext.Request.QueryString.Value} request started");
                httpContext.Response.Headers[Consts.Headers.RequestId] = contextProvider.CorrelationId;
            }

            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                using (logger.BeginScope(new Dictionary<string, object>
                       {
                           ["stackTrace"] = ex.StackTrace
                       }))
                {
                    logger.LogError(ex.Message);
                }

                throw;
            }
            finally
            {
                sw.Stop();
                using (logger.BeginScope(new Dictionary<string, object>
                       {
                           ["type"] = "incoming:http",
                           ["callerId"] = httpContext.Request.Headers["X-Caller-ID"].FirstOrDefault() ?? string.Empty,
                           ["elapsedMilliseconds"] = sw.ElapsedMilliseconds
                       }))
                {
                    logger.LogInformation($"{httpContext.Request.Method} {httpContext.Request.Path.Value}{httpContext.Request.QueryString.Value} request finished");
                }
            }
        }
    }
}
