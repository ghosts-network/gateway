using System;

namespace GhostNetwork.Gateway.Users
{
    public class User
    {
        public User(Guid id, string firstName, string lastName, string gender, DateTime? dateOfBirth, string city, string profilePicture)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            DateOfBirth = dateOfBirth;
            ProfilePicture = profilePicture;
            City = city;
        }

        public Guid Id { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Gender { get; private set; }

        public DateTime? DateOfBirth { get; private set; }

        public string ProfilePicture { get; }

        public string? City { get; private set; }

        public void Update(string gender, DateTime? dateOfBirth, string? city)
        {
            Gender = gender;
            DateOfBirth = dateOfBirth;
            City = city;
        }
    }
}