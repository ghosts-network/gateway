using GhostNetwork.Publications.Model;

namespace GhostNetwork.Gateway.Facade
{
    public interface IUpdateValidator
    {
        bool CanUpdatePublication(Publication publication);
    }
}
