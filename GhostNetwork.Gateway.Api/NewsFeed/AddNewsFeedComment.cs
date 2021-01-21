using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Gateway.Api.NewsFeed
{
    public class AddNewsFeedComment
    {
        [Required]
        public string Content { get; set; }
    }
}