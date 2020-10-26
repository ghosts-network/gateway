namespace GhostNetwork.Gateway.Api
{
    public interface ICurrentUserProvider
    {
        public string UserId { get; }
    }

    public class FakeCurrentUserProvider : ICurrentUserProvider
    {
        public string UserId => "xx-xxx-xx";
    }
}