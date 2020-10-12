using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Api.Helpers;
using GhostNetwork.Gateway.Facade;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Gateway.Api
{
    [Route("[controller]")]
    [ApiController]
    public class NewsFeedController : ControllerBase
    {
        private readonly NewsFeedPublicationsSource source;

        public NewsFeedController(NewsFeedPublicationsSource source)
        {
            this.source = source;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NewsFeedPublication>>> GetAsync()
        {
            return Ok(await source.FindManyAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NewsFeedPublication>>> GetOneAsync(string id)
        {
            return Ok(await source.FindOneAsync(id));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<NewsFeedPublication>> CreateAsync([FromBody] CreateNewsFeedPublication model)
        {
            return Ok(await source.CreateAsync(model.Content));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateAsync([FromRoute] string id, [FromBody] CreateNewsFeedPublication model)
        {
            var result = await source.UpdateAsync(id, model.Content);

            if (result.Success)
            {
                return NoContent();
            }

            if (result.Errors.Any())
            {
                return BadRequest(result.ToProblemDetails());
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAsync([FromRoute] string id)
        {
            var result = await source.DeleteAsync(id);

            if (result.Success)
            {
                return Ok();
            }

            return NotFound();
        }
    }

    public class CreateNewsFeedPublication
    {
        [Required]
        public string Content { get; set; }
    }
}
