using System;
using System.Collections.Generic;

namespace GhostNetwork.Gateway.Api.Chats;

public class UpdateChat
{
    public string Name { get; set; }

    public List<Guid> Participants { get; set; }
}