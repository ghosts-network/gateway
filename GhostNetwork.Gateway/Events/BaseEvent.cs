using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Events
{
    public abstract class BaseEvent
    {
        public string TriggeredBy { get; protected set; }
    }
}
