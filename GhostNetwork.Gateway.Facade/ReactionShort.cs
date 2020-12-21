using System.Collections.Generic;
using System.Linq;

namespace GhostNetwork.Gateway.Facade
{
    public class ReactionShort
    {
        public ReactionShort(Dictionary<ReactionType, int> reactions, ReactionType authorReaction)
        {
            Reactions = reactions
                .OrderBy(r => r.Value)
                .Select(r => r.Key)
                .ToList();

            TotalCount = reactions
                .Values
                .Sum();

            AuthorReaction = authorReaction;
        }

        public IEnumerable<ReactionType> Reactions { get; }
        public int TotalCount { get; }
        public ReactionType AuthorReaction { get; }
    }
}