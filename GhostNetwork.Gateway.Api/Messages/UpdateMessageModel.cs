using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Gateway.Api.Messages;

public class UpdateMessageModel
{
    [Required]
    public string Content { get; set; }
}