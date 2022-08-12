using System;

namespace GhostNetwork.Gateway.Messages;

public class Message
{
    public Message(string id, string chatId, UserInfo author, string content, DateTimeOffset sentOn, DateTimeOffset updatedOn)
    {
        Id = id;
        ChatId = chatId;
        Author = author;
        Content = content;
        SentOn = sentOn;
        UpdatedOn = updatedOn;
    }

    public string Id { get; init; }

    public string ChatId { get; init; }

    public UserInfo Author { get; init; }

    public string Content { get; init; }

    public DateTimeOffset SentOn { get; init; }

    public DateTimeOffset UpdatedOn { get; init; }
}