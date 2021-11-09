using System;

namespace GhostNetwork.Gateway.Users
{
    public class User
    {
        public User(Guid id, string firstName, string lastName, string gender, DateTime? dateOfBirth, string profilePicture)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            DateOfBirth = dateOfBirth;
            ProfilePicture = profilePicture;
        }

        public Guid Id { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Gender { get; private set; }

        public DateTime? DateOfBirth { get; private set; }

        public string ProfilePicture { get; }

        public void Update(string gender, DateTime? dateOfBirth)
        {
            Gender = gender;
            DateOfBirth = dateOfBirth;
        }
    }
}