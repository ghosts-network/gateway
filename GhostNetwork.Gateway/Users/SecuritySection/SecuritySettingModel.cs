using System;
using System.Linq;

namespace GhostNetwork.Gateway.Users
{
    public class SecuritySettingModel
    {
        public SecuritySettingModel(
            Guid userId,
            SecuritySettingSection friends,
            SecuritySettingSection followers)
        {
            UserId = userId;
            Friends = friends;
            Followers = followers;
        }

        public Guid UserId { get; }

        public SecuritySettingSection Followers { get; private set; }

        public SecuritySettingSection Friends { get; private set; }

        public static SecuritySettingModel DefaultForUser(Guid userId)
        {
            return new SecuritySettingModel(
                userId,
                new SecuritySettingSection(AccessLevel.Everyone, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.Everyone, Enumerable.Empty<Guid>()));
        }
    }
}
