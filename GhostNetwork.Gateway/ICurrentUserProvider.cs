using System.Threading.Tasks;

namespace GhostNetwork.Gateway
{
    public interface ICurrentUserProvider
    {
        public string UserId { get; }

        public Task<UserInfo> GetProfileAsync();
    }
}
