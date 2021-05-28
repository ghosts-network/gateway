using System.Collections.Generic;
using System.Linq;

namespace GhostNetwork.Gateway.NewsFeed
{
    public class ReactionShort
    {
        public ReactionShort(Dictionary<ReactionType, int> reactions, UserReaction user)
        {
            Reactions = reactions
                .OrderBy(r => r.Value)
                .Select(r => r.Key)
                .ToList();

            TotalCount = reactions
                .Values
                .Sum();

            User = user;
        }

        public IEnumerable<ReactionType> Reactions { get; }

        public int TotalCount { get; }

        public UserReaction User { get; }
    }
}