using System;

namespace GhostNetwork.Gateway.Api.Users
{
    public class User
    {
        public User(Guid id, string firstName, string lastName, string gender, DateTime? dateOfBirth)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            DateOfBirth = dateOfBirth;
        }

        public Guid Id { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Gender { get; }

        public DateTime? DateOfBirth { get; }
    }
}