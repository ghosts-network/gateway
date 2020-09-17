using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<IEnumerable<NewsFeedPublication>>> CreateAsync([FromBody] CreateNewsFeedPublication model)
        {
            return Ok(await source.CreateAsync(model.Content));
        }
    }

    public class CreateNewsFeedPublication
    {
        [Required]
        public string Content { get; set; }
    }
}
