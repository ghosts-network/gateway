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
        private readonly ICommentsApi commentsApi;

        public NewsFeedPublicationsSource(IPublicationsApi publicationsApi, IUpdateValidator validator, ICommentsApi commentsApi)
        {
            this.publicationsApi = publicationsApi;
            this.validator = validator;
            this.commentsApi = commentsApi;
        }
        
        public async Task<ICollection<NewsFeedPublication>> FindManyAsync()
        {
            var publications = await publicationsApi.PublicationsFindManyAsync();

            return publications.Select(p => new NewsFeedPublication(p.Content, commentsApi.CommentsFindMany(p.Id).Count)).ToList();
        }


        public async Task<NewsFeedPublication> CreateAsync(string content)
        {
            var model = new CreatePublicationModel(content);

            var result = await publicationsApi.PublicationsCreateAsync(model);

            return new NewsFeedPublication(result.Content, 0);
        }

        public async Task<(DomainResult, bool?)> UpdateAsync(string id, string content)
        {
            var publication = await publicationsApi.PublicationsFindAsync(id);

            if (publication == null)
            {
                return (DomainResult.Error("Publication not found."), false);
            }

            var result = validator.CanUpdatePublication(publication);

            if (result)
            {
                var model = new UpdatePublicationModel(content);
                await publicationsApi.PublicationsUpdateAsync(id, model);
                return (DomainResult.Successed(),true);
            }

            return (DomainResult.Error("Publication could not be updated."), false);
        }

        public async Task<DomainResult> DeleteAsync(string id)
        {
            var publication = await publicationsApi.PublicationsFindAsync(id);

            if (publication != null)
            {
                await publicationsApi.PublicationsDeleteAsync(id);
                return DomainResult.Successed();
            }

            return DomainResult.Error("Publication not found.");
        }
    }
}