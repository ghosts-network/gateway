using System.Threading.Tasks;
using GhostNetwork.Gateway.Facade;

namespace GhostNetwork.Gateway.Api
{
    public interface ICurrentUserProvider
    {
        public string UserId { get; }

        public Task<UserInfo> GetProfileAsync();
    }
}
