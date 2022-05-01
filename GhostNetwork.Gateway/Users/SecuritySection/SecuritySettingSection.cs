using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostNetwork.Gateway.Users
{
    public class SecuritySettingSection
    {
        public SecuritySettingSection(Access access, IEnumerable<Guid> certainUsers)
        {
            Access = access;
            CertainUsers = certainUsers;
        }

        public Access Access { get; }

        public IEnumerable<Guid> CertainUsers { get; }
    }
}
