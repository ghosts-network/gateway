using System;
using System.Linq;

namespace GhostNetwork.Gateway.Users
{
    public class SecuritySetting
    {
        public SecuritySetting(
            Guid userId,
            SecuritySettingSection posts,
            SecuritySettingSection friends,
            SecuritySettingSection followers,
            SecuritySettingSection comments,
            SecuritySettingSection profilePhoto)
        {
            UserId = userId;
            Posts = posts;
            Friends = friends;
            Followers = followers;
            Comments = comments;
            ProfilePhoto = profilePhoto;
        }

        public Guid UserId { get; }

        public SecuritySettingSection Posts { get; private set; }

        public SecuritySettingSection Followers { get; private set; }

        public SecuritySettingSection Friends { get; private set; }

        public SecuritySettingSection Comments { get; private set; }

        public SecuritySettingSection ProfilePhoto { get; private set; }


        public static SecuritySetting DefaultForUser(Guid userId)
        {
            return new SecuritySetting(
                userId,
                new SecuritySettingSection(Access.Everyone, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(Access.Everyone, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(Access.Everyone, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(Access.Everyone, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(Access.Everyone, Enumerable.Empty<Guid>()));
        }

        public void Update(SecuritySettingSection posts, SecuritySettingSection friends)
        {
            Posts = posts;
            Friends = friends;
        }
    }
}
