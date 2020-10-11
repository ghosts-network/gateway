using GhostNetwork.Publications.Model;

namespace GhostNetwork.Gateway.Facade
{
    public interface IUpdateValidator
    {
        DomainResult CanUpdatePublication(Publication publication);
    }
}
