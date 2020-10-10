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
        private readonly IUpdateValidator validator;

        public NewsFeedPublicationsSource(IPublicationsApi publicationsApi, IUpdateValidator validator)
        {
            this.publicationsApi = publicationsApi;
            this.validator = validator;
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

        public async Task<NewsFeedPublication> UpdateAsync(string id, string content)
        {
            var publication = await publicationsApi.PublicationsFindAsync(id);

            if (publication == null)
            {
                return null;
            }

            var result = validator.TryUpdatePublication(publication);

            if (result)
            {
                var model = new UpdatePublicationModel(content);
                await publicationsApi.PublicationsUpdateAsync(id, model);
                return new NewsFeedPublication(content);
            }

            return null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var publication = await publicationsApi.PublicationsFindAsync(id);

            if (publication != null)
            {
                await publicationsApi.PublicationsDeleteAsync(id);
                return true;
            }

            return false;
        }
    }
}