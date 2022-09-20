using System.Collections.Generic;
using System.Linq;

namespace GhostNetwork.Gateway.Api;

public class FeatureFlags
{
    private readonly IReadOnlyDictionary<string, bool> usersWithPersonalizedNewsFeed;

    public FeatureFlags(IReadOnlyCollection<string> usersWithPersonalizedNewsFeed)
    {
        this.usersWithPersonalizedNewsFeed = usersWithPersonalizedNewsFeed.ToDictionary(x => x, x => true);
    }

    public bool PersonalizedNewsFeedEnabled(string user)
    {
        return usersWithPersonalizedNewsFeed.ContainsKey(user);
    }
}