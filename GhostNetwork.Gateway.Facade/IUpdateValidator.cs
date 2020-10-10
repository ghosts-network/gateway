using System.Threading.Tasks;
using GhostNetwork.Publications.Model;

namespace GhostNetwork.Gateway.Facade
{
    public interface IUpdateValidator
    {
        bool TryUpdatePublication(Publication publication);
    }
}
