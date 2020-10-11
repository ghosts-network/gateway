using System;
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
            var publications = await publicationsApi.PublicationsSearchAsync();
            var newsFeedPublications = new List<NewsFeedPublication>();
            foreach (var publication in publications)
            {
                if (commentsApi.CommentsSearchAsyncWithHttpInfo(publication.Id).Result.Headers.TryGetValue("X-TotalCount", out var totalCount))
                {
                    if (int.TryParse(totalCount.FirstOrDefault(), out var res))
                    {
                        newsFeedPublications.Add(new NewsFeedPublication(publication.Content, res));
                    }
                    else
                    {
                        newsFeedPublications.Add(new NewsFeedPublication(publication.Content, null));
                    }
                }
            }

            return newsFeedPublications;
        }


        public async Task<NewsFeedPublication> CreateAsync(string content)
        {
            var model = new CreatePublicationModel(content);

            var result = await publicationsApi.PublicationsCreateAsync(model);

            return new NewsFeedPublication(result.Content, 0);
        }

        public async Task<NewsFeedPublication> FindOneAsync(string id)
        {
            var publication = await publicationsApi.PublicationsGetByIdAsync(id);

            if (commentsApi.CommentsSearchAsyncWithHttpInfo(id).Result.Headers.TryGetValue("X-TotalCount", out var totalCount))
            {
                if(int.TryParse(totalCount.FirstOrDefault(), out var res))
                {
                    return new NewsFeedPublication(publication.Content, res);
                }
            }

            return new NewsFeedPublication(publication.Content, null);
        }

        public async Task<DomainResult> UpdateAsync(string id, string content)
        {
            var publication = await publicationsApi.PublicationsGetByIdAsync(id);

            if (publication == null)
            {
                return DomainResult.Error("Publication not found.");
            }

            var result = validator.CanUpdatePublication(publication);

            if (result.Success)
            {
                var model = new UpdatePublicationModel(content);
                await publicationsApi.PublicationsUpdateAsync(id, model);
                return DomainResult.Successed();
            }

            return DomainResult.Error("Publication could not be updated.");
        }

        public async Task<DomainResult> DeleteAsync(string id)
        {
            var publication = await publicationsApi.PublicationsGetByIdAsync(id);

            if (publication != null)
            {
                await publicationsApi.PublicationsDeleteAsync(id);
                return DomainResult.Successed();
            }

            return DomainResult.Error("Publication not found.");
        }
    }
}