using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Gateway.Api
{
    public class CreateNewsFeedPublication
    {
        [Required]
        public string Content { get; set; }
    }
}