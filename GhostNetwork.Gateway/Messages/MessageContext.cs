using System;

namespace GhostNetwork.Gateway.Messages;

public class MessageContext
{
    public MessageContext(string chatId, Guid authorId, string content)
    {
        ChatId = chatId;
        AuthorId = authorId;
        Content = content;
    }

    public string ChatId { get; set; }

    public Guid AuthorId { get; set; }

    public string Content { get; set; }
}