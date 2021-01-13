using System;
using System.Collections.Generic;

namespace GhostNetwork.Gateway.Facade
{
    public class CommentsShort
    {
        public CommentsShort(IEnumerable<PublicationComment> topComments)
        {
            TopComments = topComments;
        }

        public IEnumerable<PublicationComment> TopComments { get; }

        [Obsolete]
        public long TotalCount = 0;
    }
}