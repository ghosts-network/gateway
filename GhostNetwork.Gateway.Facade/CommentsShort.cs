using System.Collections.Generic;

namespace GhostNetwork.Gateway.Facade
{
    public class CommentsShort
    {
        public CommentsShort(IEnumerable<PublicationComment> topComments, int totalCount)
        {
            TopComments = topComments;
            TotalCount = totalCount;
        }

        public IEnumerable<PublicationComment> TopComments { get; }
        public int TotalCount { get; }
    }
}