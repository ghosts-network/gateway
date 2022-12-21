#nullable enable
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Api.Education.EducationMaterials.FlashCards;

public interface IFlashCardsProgressManager
{
    Task<FlashCardsSetUserProgress> FindSetProgressAsync(FlashCardsSetDetails set, EducationUser user);

    Task UpdateProgressAsync(FlashCardsSetDetails set, EducationUser user, FlashCardSetTestResult results);
}
