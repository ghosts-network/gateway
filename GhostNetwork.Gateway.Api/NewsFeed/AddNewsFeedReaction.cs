using System.ComponentModel.DataAnnotations;
using GhostNetwork.Gateway.NewsFeed;

namespace GhostNetwork.Gateway.Api.NewsFeed
{
    public class AddNewsFeedReaction
    {
        [Required]
        public ReactionType Reaction { get; set; }
    }
}