using System;
using System.Collections.Generic;

namespace GhostNetwork.Gateway.Chats;

public class ChatContext
{
    public ChatContext(string name, IEnumerable<Guid> participants)
    {
        Name = name;
        Participants = participants;
    }

    public string Name { get; set; }

    public IEnumerable<Guid> Participants { get; set; }
}