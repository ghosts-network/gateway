using System.Collections.Generic;
using System.Linq;

namespace GhostNetwork.Gateway.Api;

public class FeatureFlags
{
    private readonly bool enableNewsFeedGlobally;
    private readonly IReadOnlyDictionary<string, bool> usersWithPersonalizedNewsFeed;

    public FeatureFlags(bool enableNewsFeedGlobally, IReadOnlyCollection<string> usersWithPersonalizedNewsFeed)
    {
        this.enableNewsFeedGlobally = enableNewsFeedGlobally;
        this.usersWithPersonalizedNewsFeed = usersWithPersonalizedNewsFeed.ToDictionary(x => x, x => true);
    }

    public bool PersonalizedNewsFeedEnabled(string user)
    {
        return enableNewsFeedGlobally || usersWithPersonalizedNewsFeed.ContainsKey(user);
    }
}