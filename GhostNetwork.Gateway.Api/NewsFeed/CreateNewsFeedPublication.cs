using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Gateway.Api.NewsFeed
{
    public class CreateNewsFeedPublication
    {
        [Required]
        public string Content { get; set; }
    }
}