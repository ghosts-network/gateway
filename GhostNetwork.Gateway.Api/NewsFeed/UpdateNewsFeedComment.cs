using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Gateway.Api.NewsFeed
{
    public class UpdateNewsFeedComment
    {
        [Required]
        public string Content { get; set; }
    }
}