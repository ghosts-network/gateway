using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostNetwork.Gateway.Users
{
    public class SecuritySettingSection
    {
        public SecuritySettingSection(AccessLevel access, IEnumerable<Guid> certainUsers)
        {
            Access = access;
            CertainUsers = certainUsers;
        }

        public AccessLevel Access { get; }

        public IEnumerable<Guid> CertainUsers { get; }
    }
}
