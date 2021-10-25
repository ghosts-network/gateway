using System;

namespace GhostNetwork.Gateway.Api.Users
{
    public class UpdateUserInput
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string City { get; set; }
    }
}