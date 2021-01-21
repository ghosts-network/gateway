using System;

namespace GhostNetwork.Gateway.Api.Users
{
    public class User
    {
        public User(string firstName, string lastName, string gender, DateTime? dateOfBirth)
        {
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            DateOfBirth = dateOfBirth;
        }

        public string FirstName { get; }

        public string LastName { get; }

        public string Gender { get; }

        public DateTime? DateOfBirth { get; }
    }
}