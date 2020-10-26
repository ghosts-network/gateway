using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Gateway.Api
{
    public class AddNewsFeedComment
    {
        [Required]
        public string Content { get; set; }
    }
}