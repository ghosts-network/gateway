using System;

namespace GhostNetwork.Gateway.Messages;

public class Message
{
    public string Id { get; set; }

    public string ChatId { get; set; }

    public UserInfo Author { get; set; }

    public string Content { get; set; }

    public DateTimeOffset SentOn { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }
}