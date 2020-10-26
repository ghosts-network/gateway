using System.ComponentModel.DataAnnotations;
using GhostNetwork.Gateway.Facade;

namespace GhostNetwork.Gateway.Api
{
    public class AddNewsFeedReaction
    {
        [Required]
        public ReactionType Reaction { get; set; }
    }
}