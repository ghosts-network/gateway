using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Publications.Api;

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
    }
}