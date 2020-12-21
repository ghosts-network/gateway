using System.Collections.Generic;
using System.Linq;

namespace GhostNetwork.Gateway.Facade
{
    public class ReactionShort
    {
        public ReactionShort(Dictionary<ReactionType, int> reactions, ReactionType reactionType)
        {
            Reactions = reactions
                .OrderBy(r => r.Value)
                .Select(r => r.Key)
                .ToList();

            TotalCount = reactions
                .Values
                .Sum();

            ReactionType = reactionType;
        }

        public IEnumerable<ReactionType> Reactions { get; }
        public int TotalCount { get; }
        public ReactionType ReactionType { get; }
    }
}