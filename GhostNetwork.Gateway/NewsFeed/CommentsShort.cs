using System.Collections.Generic;

namespace GhostNetwork.Gateway.NewsFeed
{
    public class CommentsShort
    {
        public CommentsShort(IEnumerable<PublicationComment> topComments, long totalCount)
        {
            TopComments = topComments;
            TotalCount = totalCount;
        }

        public IEnumerable<PublicationComment> TopComments { get; }

        public long TotalCount { get; }
    }
}