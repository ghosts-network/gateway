using System;

namespace GhostNetwork.Gateway.Users
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

        public string Gender { get; private set; }

        public DateTime? DateOfBirth { get; private set; }

        public void Update(string gender, DateTime? dateOfBirth)
        {
            Gender = gender;
            DateOfBirth = dateOfBirth;
        }
    }
}