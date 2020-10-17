using System.Collections.Generic;
using System.Linq;

namespace GhostNetwork.Gateway.Facade
{
    public class ReactionShort
    {
        public IEnumerable<ReactionType> Reactions { get; }
        public int TotalCount { get; }

        public ReactionShort(Dictionary<ReactionType, int> reactions)
        {
            Reactions = reactions
                .OrderBy(r => r.Value)
                .Select(r => r.Key)
                .ToList();

            TotalCount = reactions
                .Values
                .Sum();
        }
    }
}