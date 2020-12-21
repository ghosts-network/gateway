using System;
using System.Collections.Generic;
using System.Text;

namespace GhostNetwork.Gateway.Facade
{
    public class UserReaction
    {
        public UserReaction(ReactionType? type)
        {
            Type = type;
        }

        public ReactionType? Type { get; }
    }
}
