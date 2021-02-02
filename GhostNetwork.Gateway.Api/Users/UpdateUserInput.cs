using System;

namespace GhostNetwork.Gateway.Api.Users
{
    public class UpdateUserInput
    {
        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}