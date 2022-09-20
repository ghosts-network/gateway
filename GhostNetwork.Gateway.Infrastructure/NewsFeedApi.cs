using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GhostNetwork.Content.Model;
using Newtonsoft.Json;

namespace GhostNetwork.Gateway.Infrastructure;

public class NewsFeedApi
{
    private readonly HttpClient httpClient;

    public NewsFeedApi(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<(List<Publication>, string)> GetUserFeedAsync(string userId, int take, string cursor)
    {
        var opts = new Dictionary<string, string>
        {
            ["take"] = take.ToString()
        };

        if (cursor is not null)
        {
            opts["cursor"] = cursor;
        }

        var path = $"{userId}?{opts.Select(o => $"{o.Key}={o.Value}")}";
        var response = await httpClient.GetAsync(path);

        return (await ReadBody<List<Publication>>(response), GetCursorHeader(response));
    }

    private static async Task<TData> ReadBody<TData>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TData>(content);
    }

    private static string GetCursorHeader(HttpResponseMessage response)
    {
        return !response.Headers.TryGetValues("X-Cursor", out var headers)
            ? default
            : headers.FirstOrDefault();
    }
}