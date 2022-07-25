using System;
using System.Collections.Generic;

namespace GhostNetwork.Gateway.Api.Messages;

public class UpdateChat
{
    public string Name { get; set; }

    public IEnumerable<Guid> Participants { get; set; }
}