using System.Collections.Generic;

namespace GhostNetwork.Gateway.Chats;

public class Chat
{
    public Chat(string id, string name, IEnumerable<UserInfo> participants)
    {
        Id = id;
        Name = name;
        Participants = participants;
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<UserInfo> Participants { get; set; }
}