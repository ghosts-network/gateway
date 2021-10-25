using System;

namespace GhostNetwork.Gateway.Users
{
    public class User
    {
        public User(Guid id, string firstName, string lastName, string gender, DateTime? dateOfBirth, string city)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            DateOfBirth = dateOfBirth;
            City = city; 
        }

        public Guid Id { get; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Gender { get; private set; }

        public DateTime? DateOfBirth { get; private set; }

        public string City { get; private set; }

        public void Update(string firstName, string lastName, string gender, DateTime? dateOfBirth, string city)
        {
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            DateOfBirth = dateOfBirth;
            City = city;
        }
    }
}