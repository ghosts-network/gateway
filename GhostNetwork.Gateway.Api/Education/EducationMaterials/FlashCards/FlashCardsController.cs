#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Gateway.Api.Education.EducationMaterials.FlashCards;

[Route("/education/materials/flash-cards")]
[ApiController]
public class FlashCardsController : ControllerBase
{
    private readonly IFlashCardsCatalog catalog;
    private readonly IFlashCardsProgressManager progressManager;

    public FlashCardsController()
    {
        catalog = FlashCardsCatalog.Instance;
        progressManager = FlashCardsProgressManager.Instance;
    }

    /// <summary>
    /// Search flash card sets.
    /// </summary>
    /// <param name="cursor">Skip sets up to a specified cursor.</param>
    /// <param name="take">Take sets up to a specified amount.</param>
    /// <returns>Flash card sets.</returns>
    [HttpGet("sets")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, Consts.Headers.Cursor, "string", "")]
    public async Task<ActionResult<IEnumerable<FlashCardsSet>>> SearchSetsAsync(
        [FromQuery] string? cursor,
        [FromQuery, Range(1, 50)] int take = 20)
    {
        var (sets, nextCursor) = await catalog.FindManyAsync(new CursorPagination(cursor, take));

        if (nextCursor is null)
        {
            Response.Headers.Add(Consts.Headers.Cursor, nextCursor);
        }

        return Ok(sets);
    }

    /// <summary>
    /// Get flash cards set details.
    /// </summary>
    /// <param name="id">Flash cards set id.</param>
    /// <returns>Flash card set details.</returns>
    [HttpGet("sets/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlashCardsSetDetails>> GetSetByIdAsync([FromRoute] string id)
    {
        var set = await catalog.FindOneAsync(id);
        if (set == null)
        {
            return NotFound();
        }

        if (User.Identity?.IsAuthenticated ?? false)
        {
            set = set with { Progress = await progressManager.FindSetProgressAsync(set, new EducationUser(User.UserId()!)) };
        }

        return Ok(set);
    }

    /// <summary>
    /// Update users progress on flash card set.
    /// </summary>
    /// <param name="id">Flash card set Id.</param>
    /// <param name="results">User answers.</param>
    /// <returns>Actual user progress.</returns>
    [Authorize]
    [HttpPut("sets/{id}/progress")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlashCardsSetUserProgress>> SaveProgressAsync(
        [FromRoute] string id,
        [FromBody] FlashCardSetTestResult results)
    {
        var set = await catalog.FindOneAsync(id);
        if (set is null)
        {
            return NotFound();
        }

        await progressManager.UpdateProgressAsync(set, new EducationUser(User.UserId()!), results);

        return Ok(await progressManager.FindSetProgressAsync(set, new EducationUser(User.UserId()!)));
    }
}

internal interface IFlashCardsProgressManager
{
    Task<FlashCardsSetUserProgress> FindSetProgressAsync(FlashCardsSetDetails set, EducationUser user);

    Task UpdateProgressAsync(FlashCardsSetDetails set, EducationUser user, FlashCardSetTestResult results);
}

internal class FlashCardsProgressManager : IFlashCardsProgressManager
{
    // user - set - card
    private readonly Dictionary<string, Dictionary<string, Dictionary<string, CurrentProgressData>>> progress = new Dictionary<string, Dictionary<string, Dictionary<string, CurrentProgressData>>>();

    private readonly List<ProgressHistoryData> history = new List<ProgressHistoryData>();

    private FlashCardsProgressManager()
    {
    }

    public static FlashCardsProgressManager Instance { get; } = new FlashCardsProgressManager();

    public Task<FlashCardsSetUserProgress> FindSetProgressAsync(FlashCardsSetDetails set, EducationUser user)
    {
        if (!progress.ContainsKey(user.Id))
        {
            progress[user.Id] = new Dictionary<string, Dictionary<string, CurrentProgressData>>();
        }

        if (!progress[user.Id].ContainsKey(set.Id))
        {
            progress[user.Id][set.Id] = new Dictionary<string, CurrentProgressData>();
        }

        var fraction = set.Cards.Aggregate(0, (acc, card) => acc
                                                             + (progress[user.Id][set.Id].ContainsKey(card.Id)
                                                                 ? progress[user.Id][set.Id][card.Id].Percents
                                                                 : 0)) / (decimal)set.Cards.Count;

        return Task.FromResult(new FlashCardsSetUserProgress(fraction, set.Cards.ToDictionary(
            card => card.Id,
            card => progress[user.Id][set.Id].ContainsKey(card.Id)
                ? progress[user.Id][set.Id][card.Id].Percents
                : 0)));
    }

    public Task UpdateProgressAsync(FlashCardsSetDetails set, EducationUser user, FlashCardSetTestResult results)
    {
        var now = DateTimeOffset.UtcNow;
        history.Add(new ProgressHistoryData(set.Id, user.Id, now));
        if (!progress.ContainsKey(user.Id))
        {
            progress[user.Id] = new Dictionary<string, Dictionary<string, CurrentProgressData>>();
        }

        if (!progress[user.Id].ContainsKey(set.Id))
        {
            progress[user.Id][set.Id] = new Dictionary<string, CurrentProgressData>();
        }

        foreach (var answer in results.Answers)
        {
            var currentPercents = progress[user.Id][set.Id].ContainsKey(answer.CardId)
                ? progress[user.Id][set.Id][answer.CardId].Percents
                : 0;

            var newPercents = set.ValidateAnswer(answer)
                ? Math.Min(currentPercents + 50, 100)
                : Math.Max(currentPercents - 50, 0);

            progress[user.Id][set.Id][answer.CardId] = new CurrentProgressData(newPercents, now);
        }

        return Task.CompletedTask;
    }

    private record ProgressHistoryData(string SetId, string UserId, DateTimeOffset Date);

    private record CurrentProgressData(int Percents, DateTimeOffset LastPass);
}

/// <summary>
/// CursorPagination limit object for response result.
/// </summary>
/// <param name="Cursor">Search ignore all elements before cursor including.</param>
/// <param name="Take">Takes specified amount of elements if available.</param>
public record CursorPagination(string? Cursor, int Take);

public interface IFlashCardsCatalog
{
    Task<(IReadOnlyCollection<FlashCardsSet>, string?)> FindManyAsync(CursorPagination pagination);

    Task<FlashCardsSetDetails?> FindOneAsync(string id);
}

internal class FlashCardsCatalog : IFlashCardsCatalog
{
    private FlashCardsCatalog()
    {
        var json = File.ReadAllText("catalog.json");
        Sets = JsonSerializer.Deserialize<List<FlashCardsSetDetails>>(json, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        })!;
    }

    public static FlashCardsCatalog Instance { get; } = new FlashCardsCatalog();

    private IReadOnlyCollection<FlashCardsSetDetails> Sets { get; }

    public Task<(IReadOnlyCollection<FlashCardsSet>, string?)> FindManyAsync(CursorPagination pagination)
    {
        IReadOnlyCollection<FlashCardsSet> list = Sets
            .Select(s => new FlashCardsSet(s.Id, s.Title, new FlashCardsSetInfo(s.Cards.Count)))
            .ToList();

        return Task.FromResult((list, default(string?)));
    }

    public Task<FlashCardsSetDetails?> FindOneAsync(string id)
    {
        return Task.FromResult(Sets.FirstOrDefault(s => s.Id == id));
    }
}
