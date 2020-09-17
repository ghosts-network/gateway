using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Publications.Api;
using GhostNetwork.Publications.Model;

namespace GhostNetwork.Gateway.Facade
{
    public class NewsFeedPublicationsSource
    {
        private readonly IPublicationsApi publicationsApi;

        public NewsFeedPublicationsSource(IPublicationsApi publicationsApi)
        {
            this.publicationsApi = publicationsApi;
        }
        
        public async Task<ICollection<NewsFeedPublication>> FindManyAsync()
        {
            var publications = await publicationsApi.PublicationsFindManyAsync();

            return publications.Select(p => new NewsFeedPublication(p.Content)).ToList();
        }

        public async Task<NewsFeedPublication> CreateAsync(string content)
        {
            var model = new CreatePublicationModel(content);

            var result = await publicationsApi.PublicationsCreateAsync(model);

            return new NewsFeedPublication(result.Content);
        }
    }
}