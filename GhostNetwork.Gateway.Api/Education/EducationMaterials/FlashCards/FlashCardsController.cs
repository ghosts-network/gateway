#nullable enable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Education.Api;
using GhostNetwork.Education.Client;
using GhostNetwork.Education.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Gateway.Api.Education.EducationMaterials.FlashCards;

[Route("/education/materials/flash-cards")]
[ApiController]
public class FlashCardsController : ControllerBase
{
    private readonly IFlashCardsApi flashCardsApi;

    public FlashCardsController(IFlashCardsApi flashCardsApi)
    {
        this.flashCardsApi = flashCardsApi;
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
        var response = await flashCardsApi.SearchSetsWithHttpInfoAsync(cursor, take);

        if (response.Headers.TryGetValue("X-Cursor", out var headers))
        {
            Response.Headers.Add(Consts.Headers.Cursor, headers.FirstOrDefault());
        }

        return Ok(response.Data);
    }

    /// <summary>
    /// Get flash cards set details.
    /// </summary>
    /// <param name="id">Flash cards set id.</param>
    /// <returns>Flash card set details.</returns>
    [HttpGet("sets/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlashCardsSetDetailsWithProgressViewModel>> GetSetByIdAsync([FromRoute] string id)
    {
        try
        {
            var response = await flashCardsApi.GetSetByIdWithHttpInfoAsync(id, User.UserId());

            return Ok(response.Data);
        }
        catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
        {
            return NotFound();
        }
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
        try
        {
            var response = await flashCardsApi.SaveProgressWithHttpInfoAsync(id, User.UserId()!, results);

            return Ok(response.Data);
        }
        catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.BadRequest)
        {
            return BadRequest();
        }
    }
}
