using System;
using System.Collections.Generic;

namespace GhostNetwork.Gateway.Users.SecuritySection
{
    public class SecuritySettingsSectionInputModel
    {
        public AccessLevel Access { get; set; }

        public IEnumerable<Guid> CertainUsers { get; set; }
    }
}
