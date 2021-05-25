using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Facade
{
    public interface ICurrentUserProvider
    {
        public string UserId { get; }

        public Task<UserInfo> GetProfileAsync();
    }
}
